using System.Collections.Generic;


namespace CoreModels.XyApi.JingDong
{
    public class jdWaybillCodeGet  //获取京东快递运单号
    {
        public jdWaybillCodeGetResponce jingdong_etms_waybillcode_get_responce { get; set; }
    }

    public class jdWaybillCodeGetResponce {
        public jdWaybillCodeGetResultInfo resultInfo { get; set; }
    }
    public class jdWaybillCodeGetResultInfo {
        public List<string> deliveryIdList;
    }

    public class jdTraceGetModel //查询京东快递物流跟踪信息
    {
        public jdTraceGetModelResponce jingdong_etms_trace_get_responce { get; set; }
    }
    public class jdTraceGetModelResponce {
        public List<trace_api_dto> trace_api_dtos { get; set; }
    }

    public class trace_api_dto
    {
        public string ope_remark { get; set; }
        public string ope_time { get; set; }
        public string ope_title { get; set; }
        public string ope_name { get; set; }
    }

    public class jdPackageUpdateModel // 修改京东快递包裹数
    {
        public jdPackageUpdateModelResponse jingdong_etms_package_update_responce { get; set; }
    }

    public class jdPackageUpdateModelResponse
    {
        public jdPackageUpdateModelResponseRe response { get; set; }
    }

    public class jdPackageUpdateModelResponseRe {
        public string stateMessage { get; set; }
        public string stateCode { get; set; }
    }

    public class ajaxExpressModel { //获取可选择的快递
        public List<expressInfo> data { get; set; }
    }

    public class expressInfo {
        public string exname { get; set; }
        public string ico { get; set; }
        public string name { get; set; }
        public string price { get; set; }
        public string type { get; set; }
        public string wastetime { get; set; }
    }

    public class jdWaybillSendModel { //京东快递提交运单信息接口（青龙接单接口）
        public jdWaybillSendModelResponse jingdong_etms_waybill_send_responce { get; set; }
    }
    public class jdWaybillSendModelResponse {
        public jdWaybillSendModelResponseResultInfo resultInfo { get; set; }
    }
    public class jdWaybillSendModelResponseResultInfo {
        public string message { get; set; }
        public string deliveryId { get; set; }
        public string code { get; set; }
        public string orderId { get; set; }
    }

    public class jdOrderPrintModel {// 获取京东快递运单打印
        public jdOrderPrintModelResponse jingdong_etms_order_print_responce;

    }
    public class jdOrderPrintModelResponse {
        public jdOrderPrintModelResponseRe response { get; set; }
    }
    public class jdOrderPrintModelResponseRe {
        public string stateMessage { get; set; }
        public string pdfArr { get; set; }
        public string stateCode { get; set; }
    }

    public class jdEportSendModel { //接收Eport相关扩展信息
        public jdEportSendModelResponse jingdong_ldop_receive_eport_send_responce { get; set; }
    }

    public class jdEportSendModelResponse {
        public jdEportSendModelResponseResult receiveextenmessagetoeport_result { get; set; }
    }
    public class jdEportSendModelResponseResult {
        public string resultCode { get; set; }
        public string resultMessage { get; set; }
    }

    public class jdOrderInterceptModel { //订单拦截
        public jdOrderInterceptModelResponse jingdong_ldop_receive_order_intercept_response { get; set; }

    }

    public class jdOrderInterceptModelResponse {
        public jdOrderInterceptModelResponseResultInfo resultInfo { get; set; }
    }

    public class jdOrderInterceptModelResponseResultInfo {
        public string stateMessage { get; set; }
        public string stateCode { get; set; }
    }

    public class jdRangeCheckModel
    { //检查是否京配

        public jdRangeCheckModelResponse jingdong_etms_range_check_responce { get; set; }
    }

    public class jdRangeCheckModelResponse
    {
        public jdRangeCheckModelResponseResultInfo resultInfo { get; set; }
    }
    public class jdRangeCheckModelResponseResultInfo
    {
        public string agingName { get; set; }
        public string targetSortCenterId { get; set; }
        public string sourcetSortCenterName { get; set; }
        public string rcode { get; set; }
        public string rmessage { get; set; }
        public string sourcetSortCenterId { get; set; }
        public string road { get; set; }
        public string originalTabletrolleyCode { get; set; }
        public string destinationCrossCode { get; set; }
        public string siteId { get; set; }
        public string originalCrossCode { get; set; }
        public string destinationTabletrolleyCode { get; set; }
        public string targetSortCenterName { get; set; }
        public string siteName { get; set; }
        public string aging { get; set; }
    }












}
