using System;
using System.Collections.Generic;

namespace CoreModels.XyComm
{
     public class Area
     {
        public int ID{get;set;}
		public int ParentId{get;set;}
		public string Name{get;set;}
		public string MergerName{get;set;}
		public string ShortName{get;set;}
		public string MergerShortName{get;set;}
		public string LevelType{get;set;}
		public string CityCode{get;set;}
		public string ZipCode{get;set;}
		public string Pinyin{get;set;}
		public string Jianpin{get;set;}
		public string FirstChar{get;set;}
     }
}