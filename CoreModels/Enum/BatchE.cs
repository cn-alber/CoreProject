namespace CoreModels.Enum
{
    public class BatchE
    {
        public enum BatchStatus
        {
            等待拣货 = 0,
            等待播种 = 1,
            等待出库验证 = 2,
            正在拣货 = 3,
            正在播种 = 4,
            正在出库验证 = 5,
            已完成   = 6,
            终止拣货 = 7
        }

        public enum BatchType
        {
            一单一件 = 0,
            一单多件 = 1,
            现场大单 = 2,
            整箱 = 3,
            补货 = 4,
            组团 = 5,
            采购退货 = 6
        }
    }
}