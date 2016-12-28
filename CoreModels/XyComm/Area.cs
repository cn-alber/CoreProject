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
	
	 public class AreaQuery{
		 public int ID{get;set;}
		 public int ParentId{get;set;}
		 public string Name{get;set;}
		 public string LevelType{get;set;}
	 }

	 public class AreaAll{
		 public int value{get;set;}
		 public string label{get;set;}
		 public int ParentId{get;set;}
		 public List<AreaAll> children{get;set;}
	 }

	 public class AreaCascader{
		 public int value{get;set;}
		 public string label{get;set;}
		 public List<AreaCascader2> children{get;set;}

	 }

	public class AreaCascader2{
		 public int value{get;set;}
		 public string label{get;set;}
		 public List<AreaCascader3> children{get;set;}

	 }
	public class AreaCascader3{
		 public int value{get;set;}
		 public string label{get;set;}

	 }

}