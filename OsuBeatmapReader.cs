using System;
using System.Collections.Generic;
using System.IO;

namespace OsuBeatmapReader
{
    internal class OsuBeatmapReader
    {
        /// <summary>
        /// All Data, as full String
        /// </summary>
        public string RAW_Data;

        public int OsuFileFormat;
        public string BackgroundFileName;

        /// <summary>
        /// File Read Error Message
        /// </summary>
        public string Error;

        #region General

        public string AudioFileName;
        public int AudioPreviewTime;

        /// <summary>
        /// GameMode: 1=osu , 2=taiko , 3=catch , 4=mania
        /// </summary>
        public int GameMode;

        #endregion General

        #region Metadata

        public string Title;
        public string TitleUnicode;
        public string Artist;
        public string ArtistUnicde;
        public string  Creator;
        public string Version;
        public string Source;
        public string Tags;
        public int BeatmapID;
        public int BeatmapSetID;
       
        #endregion Metadata

        #region Difficulty

        public float HP;
        public float CS;
        public float OD;
        public float AR;
        public float SliderMultiplier;
        public float SliderTickrate;

        #endregion Difficulty

        public List<int> Circle_PosX;
        public List<int> Circle_PosY;
        public List<int> Circle_Time;

        // Sliders are not supported yet.

        ///<summary>
        /// Get osu! Beatmap Data (string: .osu path)
        ///</summary>
        public void GetBeatmapData(string path)
        {
            if (Circle_PosX == null)
            {
            }
            else
            {
                Circle_PosX.Clear();
            }

            if (Circle_PosY == null)
            {
            }
            else
            {
                Circle_PosY.Clear();
            }

            if (Circle_Time == null)
            {
            }
            else
            {
                Circle_Time.Clear();
            }

            BackgroundFileName = "";
            RAW_Data = "";
            OsuFileFormat = -1;
            AudioFileName = "";
            AudioPreviewTime = -1;
            GameMode = -1;
            Title = "";
            TitleUnicode = "";
            Artist = "";
            ArtistUnicde = "";
            Creator = "";
            Version = "";
            Source = "";
            Tags = "";
            BeatmapID = -1;
            BeatmapSetID = -1;
            HP = -1;
            CS = -1;
            OD = -1;
            AR = -1;
            SliderMultiplier = -1;
            SliderTickrate = -1;

            string bmpath = path;
            try
            {
                FileStream fs = new FileStream(bmpath, FileMode.Open);
                StreamReader streamReader = new StreamReader(fs);

                bool GeneralTag= false;
                bool MetaDataTag = false;
                bool DifficultyTag = false;
                bool EventsTag = false;
                bool TimingPointsTag = false;
                bool HitObjectsTag = false;

                while (streamReader.Peek() != -1)
                {
                    string str = streamReader.ReadLine();

                    if (str.StartsWith("//"))
                    {
                        //Skip Line
                    }
                    else
                    {
                        if ((GeneralTag == false) && (MetaDataTag == false) && (DifficultyTag == false) && (EventsTag == false) && (TimingPointsTag == false) && (HitObjectsTag == false))
                        {
                            if (str.StartsWith("osu file format v"))
                            {
                                OsuFileFormat = Convert.ToInt32(str.Replace("osu file format v", ""));
                            }
                        }

                        if (str.Equals("[General]"))
                        {
                            GeneralTag = true;
                        }
                        if (str.Equals("[Metadata]"))
                        {
                            GeneralTag = false;
                            MetaDataTag = true;
                        }
                        if (str.Equals("[Difficulty]"))
                        {
                            MetaDataTag = false;
                            DifficultyTag = true;
                        }

                        if (str.Equals("[Events]"))
                        {
                            DifficultyTag = false;
                            EventsTag = true;
                        }

                        if (str.Equals("[TimingPoints]"))
                        {
                            EventsTag = false;
                            TimingPointsTag = true;
                        }

                        if (str.Equals("[HitObjects]"))
                        {
                            TimingPointsTag = false;
                            HitObjectsTag = true;
                        }
                        if (GeneralTag)
                        {
                            if (str.StartsWith("AudioFilename:"))
                            {
                                RAW_Data = RAW_Data + str + Environment.NewLine;
                                AudioFileName = Convert.ToString(str.Replace("AudioFilename: ", ""));
                            }

                            if (str.StartsWith("PreviewTime:"))
                            {
                                RAW_Data = RAW_Data + str + Environment.NewLine;
                                AudioPreviewTime = Convert.ToInt32(str.Replace("AudioFilename:", ""));
                            }

                            if (str.StartsWith("Mode:"))
                            {
                                RAW_Data = RAW_Data + str + Environment.NewLine;
                                GameMode = Convert.ToInt32(str.Replace("Mode:", ""));
                            }
                        }

                        if (MetaDataTag)
                        {
                            if (str.StartsWith("Title:"))
                            {
                                RAW_Data = RAW_Data + str + Environment.NewLine;
                                Title = Convert.ToString(str.Replace("Title:", ""));
                            }
                            if (str.StartsWith("TitleUnicode:"))
                            {
                                RAW_Data = RAW_Data + str + Environment.NewLine;
                                TitleUnicode = Convert.ToString(str.Replace("TitleUnicode:", ""));
                            }
                            if (str.StartsWith("Artist:"))
                            {
                                RAW_Data = RAW_Data + str + Environment.NewLine;
                                Artist = Convert.ToString(str.Replace("Artist:", ""));
                            }
                            if (str.StartsWith("ArtistUnicde:"))
                            {
                                RAW_Data = RAW_Data + str + Environment.NewLine;
                                ArtistUnicde = Convert.ToString(str.Replace("ArtistUnicde:", ""));
                            }
                            if (str.StartsWith("Creator:"))
                            {
                                RAW_Data = RAW_Data + str + Environment.NewLine;
                                Creator = Convert.ToString(str.Replace("Creator:", ""));
                            }
                            if (str.StartsWith("Version:"))
                            {
                                RAW_Data = RAW_Data + str + Environment.NewLine;
                                Version = Convert.ToString(str.Replace("Version:", ""));
                            }
                            if (str.StartsWith("Source:"))
                            {
                                RAW_Data = RAW_Data + str + Environment.NewLine;
                                Source = Convert.ToString(str.Replace("Source:", ""));
                            }
                            if (str.StartsWith("Tags:"))
                            {
                                RAW_Data = RAW_Data + str + Environment.NewLine;
                                Tags = Convert.ToString(str.Replace("Tags:", ""));
                            }
                            if (str.StartsWith("BeatmapID:"))
                            {
                                RAW_Data = RAW_Data + str + Environment.NewLine;
                                BeatmapID = Convert.ToInt32(str.Replace("BeatmapID:", ""));
                            }
                            if (str.StartsWith("BeatmapSetID:"))
                            {
                                RAW_Data = RAW_Data + str + Environment.NewLine;
                                BeatmapSetID = Convert.ToInt32(str.Replace("BeatmapSetID:", ""));
                            }
                        }

                        if (DifficultyTag)
                        {
                            if (str.StartsWith("HPDrainRate:"))
                            {
                                RAW_Data = RAW_Data + str + Environment.NewLine;
                                HP = Convert.ToSingle(str.Replace("HPDrainRate:", ""));
                            }
                            if (str.StartsWith("CircleSize:"))
                            {
                                RAW_Data = RAW_Data + str + Environment.NewLine;
                                CS = Convert.ToSingle(str.Replace("CircleSize:", ""));
                            }
                            if (str.StartsWith("OverallDifficulty:"))
                            {
                                RAW_Data = RAW_Data + str + Environment.NewLine;
                                OD = Convert.ToSingle(str.Replace("OverallDifficulty:", ""));
                            }
                            if (str.StartsWith("ApproachRate:"))
                            {
                                RAW_Data = RAW_Data + str + Environment.NewLine;
                                AR = Convert.ToSingle(str.Replace("ApproachRate:", ""));
                            }
                            if (str.StartsWith("SliderMultiplier:"))
                            {
                                RAW_Data = RAW_Data + str + Environment.NewLine;
                                SliderMultiplier = Convert.ToSingle(str.Replace("SliderMultiplier:", ""));
                            }
                            if (str.StartsWith("SliderTickRate:"))
                            {
                                RAW_Data = RAW_Data + str + Environment.NewLine;
                                SliderTickrate = Convert.ToSingle(str.Replace("SliderTickRate:", ""));
                            }
                        }
                        if (EventsTag)
                        {
                            string[] strArray = str.Split(',');
                            for (int index = 0; index < strArray.Length; ++index)
                            {
                                if (index == 2)
                                {
                                    char checkForFile = '"';
                                    if ((strArray[index].StartsWith("" + checkForFile)) && (strArray[index].EndsWith("" + checkForFile)))
                                    {
                                        BackgroundFileName = strArray[index];
                                        EventsTag = false;
                                        TimingPointsTag = true;
                                    }
                                    
                                }
                            }
                        }
                        if (TimingPointsTag)
                        {
                            // Soon...
                        }

                        if (HitObjectsTag)
                        {
                            if (str.Contains("|"))
                            {
                            }
                            else
                            {
                                string[] strArray = str.Split(',');
                                for (int index = 0; index < strArray.Length; ++index)
                                {
                                    if (index == 0)
                                    {
                                        Circle_PosX.Add(Convert.ToInt32(strArray[index]));
                                    }

                                    if (index == 1)
                                    {
                                        Circle_PosY.Add(Convert.ToInt32(strArray[index]));
                                    }
                                    if (index == 2)
                                    {
                                        Circle_Time.Add(Convert.ToInt32(strArray[index]));
                                    }
                                }
                            }
                        }
                    }
                }
                streamReader.Close();
                fs.Close();
                if (GameMode == -1)
                {
                    GameMode = 0;
                }
            }
            catch (Exception ex)
            {
                this.Error = ex.Message + Environment.NewLine + ex.InnerException + Environment.NewLine + ex.Source;
            }
        }
    }
}