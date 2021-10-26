using System;
using System.Collections;

namespace UnicodeSrl.ScciCore.DifferenceEngine
{
    public enum DiffEngineLevel
    {
        FastImperfect,
        Medium,
        SlowPerfect
    }

    public class DiffEngine
    {
        private IDiffList _source;
        private IDiffList _dest;
        private ArrayList _matchList;

        private DiffEngineLevel _level;

        private DiffStateList _stateList;

        public DiffEngine()
        {
            _source = null;
            _dest = null;
            _matchList = null;
            _stateList = null;
            _level = DiffEngineLevel.FastImperfect;
        }

        private int GetSourceMatchLength(int destIndex, int sourceIndex, int maxLength)
        {
            int matchCount;
            for (matchCount = 0; matchCount < maxLength; matchCount++)
            {
                if (_dest.GetByIndex(destIndex + matchCount).CompareTo(_source.GetByIndex(sourceIndex + matchCount)) != 0)
                {
                    break;
                }
            }
            return matchCount;
        }

        private void GetLongestSourceMatch(DiffState curItem, int destIndex, int destEnd, int sourceStart, int sourceEnd)
        {

            int maxDestLength = (destEnd - destIndex) + 1;
            int curLength = 0;
            int curBestLength = 0;
            int curBestIndex = -1;
            int maxLength = 0;
            for (int sourceIndex = sourceStart; sourceIndex <= sourceEnd; sourceIndex++)
            {
                maxLength = Math.Min(maxDestLength, (sourceEnd - sourceIndex) + 1);
                if (maxLength <= curBestLength)
                {
                    break;
                }
                curLength = GetSourceMatchLength(destIndex, sourceIndex, maxLength);
                if (curLength > curBestLength)
                {
                    curBestIndex = sourceIndex;
                    curBestLength = curLength;
                }
                sourceIndex += curBestLength;
            }
            if (curBestIndex == -1)
            {
                curItem.SetNoMatch();
            }
            else
            {
                curItem.SetMatch(curBestIndex, curBestLength);
            }

        }

        private void ProcessRange(int destStart, int destEnd, int sourceStart, int sourceEnd)
        {
            int curBestIndex = -1;
            int curBestLength = -1;
            int maxPossibleDestLength = 0;
            DiffState curItem = null;
            DiffState bestItem = null;
            for (int destIndex = destStart; destIndex <= destEnd; destIndex++)
            {
                maxPossibleDestLength = (destEnd - destIndex) + 1;
                if (maxPossibleDestLength <= curBestLength)
                {
                    break;
                }
                curItem = _stateList.GetByIndex(destIndex);

                if (!curItem.HasValidLength(sourceStart, sourceEnd, maxPossibleDestLength))
                {
                    GetLongestSourceMatch(curItem, destIndex, destEnd, sourceStart, sourceEnd);
                }
                if (curItem.Status == DiffStatus.Matched)
                {
                    switch (_level)
                    {
                        case DiffEngineLevel.FastImperfect:
                            if (curItem.Length > curBestLength)
                            {
                                curBestIndex = destIndex;
                                curBestLength = curItem.Length;
                                bestItem = curItem;
                            }
                            destIndex += curItem.Length - 1;
                            break;
                        case DiffEngineLevel.Medium:
                            if (curItem.Length > curBestLength)
                            {
                                curBestIndex = destIndex;
                                curBestLength = curItem.Length;
                                bestItem = curItem;
                                destIndex += curItem.Length - 1;
                            }
                            break;
                        default:
                            if (curItem.Length > curBestLength)
                            {
                                curBestIndex = destIndex;
                                curBestLength = curItem.Length;
                                bestItem = curItem;
                            }
                            break;
                    }
                }
            }
            if (curBestIndex < 0)
            {
            }
            else
            {

                int sourceIndex = bestItem.StartIndex;
                _matchList.Add(DiffResultSpan.CreateNoChange(curBestIndex, sourceIndex, curBestLength));
                if (destStart < curBestIndex)
                {
                    if (sourceStart < sourceIndex)
                    {
                        ProcessRange(destStart, curBestIndex - 1, sourceStart, sourceIndex - 1);
                    }
                }
                int upperDestStart = curBestIndex + curBestLength;
                int upperSourceStart = sourceIndex + curBestLength;
                if (destEnd > upperDestStart)
                {
                    if (sourceEnd > upperSourceStart)
                    {
                        ProcessRange(upperDestStart, destEnd, upperSourceStart, sourceEnd);
                    }
                }
            }
        }

        public double ProcessDiff(IDiffList source, IDiffList destination, DiffEngineLevel level)
        {
            _level = level;
            return ProcessDiff(source, destination);
        }

        public double ProcessDiff(IDiffList source, IDiffList destination)
        {
            DateTime dt = DateTime.Now;
            _source = source;
            _dest = destination;
            _matchList = new ArrayList();

            int dcount = _dest.Count();
            int scount = _source.Count();


            if ((dcount > 0) && (scount > 0))
            {
                _stateList = new DiffStateList(dcount);
                ProcessRange(0, dcount - 1, 0, scount - 1);
            }

            TimeSpan ts = DateTime.Now - dt;
            return ts.TotalSeconds;
        }


        private bool AddChanges(
            ArrayList report,
            int curDest,
            int nextDest,
            int curSource,
            int nextSource)
        {
            bool retval = false;
            int diffDest = nextDest - curDest;
            int diffSource = nextSource - curSource;
            int minDiff = 0;
            if (diffDest > 0)
            {
                if (diffSource > 0)
                {
                    minDiff = Math.Min(diffDest, diffSource);
                    report.Add(DiffResultSpan.CreateReplace(curDest, curSource, minDiff));
                    if (diffDest > diffSource)
                    {
                        curDest += minDiff;
                        report.Add(DiffResultSpan.CreateAddDestination(curDest, diffDest - diffSource));
                    }
                    else
                    {
                        if (diffSource > diffDest)
                        {
                            curSource += minDiff;
                            report.Add(DiffResultSpan.CreateDeleteSource(curSource, diffSource - diffDest));
                        }
                    }
                }
                else
                {
                    report.Add(DiffResultSpan.CreateAddDestination(curDest, diffDest));
                }
                retval = true;
            }
            else
            {
                if (diffSource > 0)
                {
                    report.Add(DiffResultSpan.CreateDeleteSource(curSource, diffSource));
                    retval = true;
                }
            }
            return retval;
        }

        public ArrayList DiffReport()
        {
            ArrayList retval = new ArrayList();
            int dcount = _dest.Count();
            int scount = _source.Count();

            if (dcount == 0)
            {
                if (scount > 0)
                {
                    retval.Add(DiffResultSpan.CreateDeleteSource(0, scount));
                }
                return retval;
            }
            else
            {
                if (scount == 0)
                {
                    retval.Add(DiffResultSpan.CreateAddDestination(0, dcount));
                    return retval;
                }
            }


            _matchList.Sort();
            int curDest = 0;
            int curSource = 0;
            DiffResultSpan last = null;

            foreach (DiffResultSpan drs in _matchList)
            {
                if ((!AddChanges(retval, curDest, drs.DestIndex, curSource, drs.SourceIndex)) &&
                    (last != null))
                {
                    last.AddLength(drs.Length);
                }
                else
                {
                    retval.Add(drs);
                }
                curDest = drs.DestIndex + drs.Length;
                curSource = drs.SourceIndex + drs.Length;
                last = drs;
            }

            AddChanges(retval, curDest, dcount, curSource, scount);

            return retval;
        }
    }
}
