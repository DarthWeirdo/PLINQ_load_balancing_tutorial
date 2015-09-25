using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace PLINQ_load_balancing
{
    class MyPartitioner : Partitioner<FileInZip>
    {
        private readonly List<FileInZip> _filesInZips;

        public MyPartitioner(List<FileInZip> filesInZips)
        {            
            _filesInZips = new List<FileInZip>(filesInZips.OrderBy(p => p.ParentZip));
        }

        public override bool SupportsDynamicPartitions
        {
            get { return false; }
        }

        public override IList<IEnumerator<FileInZip>> GetPartitions(int partitionCount)
        {           
            List<IEnumerator<FileInZip>> result = new List<IEnumerator<FileInZip>>();
            
            int start = 0;
            int end = 0;
            long size = 0;
            int pCounter = 0;
            
            if (partitionCount > _filesInZips.Count)
            {
                for (int i = 0; i < partitionCount; i++)
                {
                    result.Add(i <= _filesInZips.Count - 1
                        ? GetItemsForPartition(_filesInZips, i, i)
                        : GetItemsForPartition(new List<FileInZip> {new FileInZip(null, null, 0)}, 0, 0));
                }
                return result;
            }

            if (partitionCount > 2)
            {
                // fill first (n - 2) chunks
                long chunkSize = _filesInZips.Sum(file => file.Size) / partitionCount;

                foreach (FileInZip file in _filesInZips)
                {
                    size += file.Size;
                    if (size > chunkSize)
                    {
                        result.Add(GetItemsForPartition(_filesInZips, start, end));                        
                        start = end + 1;
                        size = 0;
                        pCounter++;
                    }
                    end++;

                    if (pCounter == partitionCount - 2 || end == _filesInZips.Count - 3)
                        break;                    
                }
                // hungry algorithm for the last two chunks
                List<FileInZip> rest = _filesInZips.GetRange(start, _filesInZips.Count - start);
                FillTwoChunks(rest, result);
            }
            
            else switch (partitionCount)
            {
                case 2:
                    FillTwoChunks(_filesInZips, result);
                    break;
                case 1:
                    result.Add(GetItemsForPartition(_filesInZips, 0, _filesInZips.Count - 1));
                    break;
                default:
                    if (partitionCount <= 0)
                        throw new ArgumentOutOfRangeException();
                    break;
            }

            return result;
        }

        
        /// <summary>
        /// Hungry algorithm for two chunks
        /// </summary>        
        private void FillTwoChunks(List<FileInZip> source, List<IEnumerator<FileInZip>> list)
        {                        
            List<FileInZip> firstChunk = new List<FileInZip>();
            List<FileInZip> secondChunk = new List<FileInZip>();

            foreach (FileInZip file in source)
            {
                long sumA = firstChunk.Sum(fileA => fileA.Size);
                long sumB = secondChunk.Sum(fileB => fileB.Size);

                if (sumA < sumB)
                    firstChunk.Add(file);
                else
                    secondChunk.Add(file);
            }
            list.Add(GetItemsForPartition(firstChunk, 0, firstChunk.Count - 1));
            list.Add(GetItemsForPartition(secondChunk, 0, secondChunk.Count - 1));
        }


        IEnumerator<FileInZip> GetItemsForPartition(List<FileInZip> list, int start, int end)
        {
            for (int i = start; i <= end; i++)
                yield return list[i];
        }
    }
}