using System.Collections.Generic;

namespace CoreModels.XyApi.JingDong
{
    public class jdRefundListResponce {//退款审核单列表查询
        public jdRefundListResponce1 jingdong_pop_afs_soa_refundapply_queryPageList_responce { get; set; }
    }

    public class jdRefundListResponce1
    {
        public string code { get; set; }
        public jdRefundListQueryresult queryResult { get; set; }

    }

    public class jdRefundListQueryresult
    {
        public List<jdRefundListresult> result { get; set; }
        public int totalCount { get; set; }
    }


    public class jdRefundListresult
    {
        public string applyRefundSum { get; set; }
        public string applyTime { get; set; }
        public string buyerId { get; set; }
        public string buyerName { get; set; }
        public string checkTime { get; set; }
        public string checkUserName { get; set; }
        public string id { get; set; }
        public string orderId { get; set; }
        public string status { get; set; }
    }

    public class jdRefundQueryById { //根据Id查询退款审核单
        public jingdong_pop_afs_soa_refundapply_queryById_response jingdong_pop_afs_soa_refundapply_queryById_responce { get; set; }
    }

    public class jingdong_pop_afs_soa_refundapply_queryById_response {
        public jdRefundQueryByIdQueryresult queryResult { get; set; }

    }
    public class jdRefundQueryByIdQueryresult {

        public List<jdRefundQueryByIdqueryResultR> result { get; set; }
        public int totalCount { get; set; }
        public string errorMsg { get; set; }

    }

    public class jdRefundQueryByIdqueryResultR {
        public refundApplyVo refundApplyVo { get; set; }
    }

    public class refundApplyVo
    {
        public string id { get; set; }
        public string checkRemark { get; set; }
        public string status { get; set; }
        public string applyTime { get; set; }
        public string applyRefundSum { get; set; }
        public string checkTime { get; set; }
        public string buyerId { get; set; }
        public string orderId { get; set; }
        public string buyerName { get; set; }
        public string checkUserName { get; set; }

    }

    public class jdReplyRefundModel { //商家审核退款单
        public jdReplyRefundModelResponse jingdong_pop_afs_soa_refundapply_replyRefund_responce { get; set; }

    }

    public class jdReplyRefundModelResponse
    {
        public jdReplyRefundReplyResult replyResult { get; set; }

    }

    public class jdReplyRefundReplyResult
    {
        public string success { get; set; }
    }

    public class jdGetWaitRefundNumModel{ //待处理退款单数查询
        public jdGetWaitRefundNumModelResponce jingdong_pop_afs_soa_refundapply_getWaitRefundNum_responce { get; set; }
    }

    public class jdGetWaitRefundNumModelResponce {
        public jdGetWaitRefundNumModelQueryResult queryResult { get; set; }
    }
    public class jdGetWaitRefundNumModelQueryResult
    {
        public string success { get; set; }
        public int totalCount { get; set; } 
    }

   




}
