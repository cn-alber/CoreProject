
namespace CoreModels.XyApi.JingDong
{
    public class jdDpsOutboundModel // 厂商直送出库
    {
        public jdDpsOutboundModelResponse jingdong_dropship_dps_outbound_responce { get; set; }
    }

    public class jdDpsOutboundModelResponse {
        public jdDpsOutboundModelResponseOutBoundResult outBoundResult { get; set; }
    }

    public class jdDpsOutboundModelResponseOutBoundResult {
        public string message { get; set; }

    }

    public class jdDpsDeliveryModel { //厂商直送发货
        public jdDpsDeliveryModelResponse jingdong_dropship_dps_delivery_responce { get; set; }
    }
    public class jdDpsDeliveryModelResponse {
        public jdDpsDeliveryModelResponseDeliverResult deliverResult { get; set; }
    }
    public class jdDpsDeliveryModelResponseDeliverResult {
        public string message { get; set; }
    }

    public class jdEptDeliveryOrderModel{  //ept 订单发货
        public jdEptDeliveryOrderModelResponse jingdong_ept_order_deliveryorder_responce { get; set; }
    }

    public class jdEptDeliveryOrderModelResponse {
        public deliveryorder_result deliveryorder_result { get; set; }
    }
    public class deliveryorder_result {
        public string success { get; set; }
    }


}
