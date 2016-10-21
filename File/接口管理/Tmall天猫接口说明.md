接口文件：CoreWebApi/Controllers/Api/Tmall/TmTtradesControllers.cs

function说明
==============
1.soldget()            下载订单

 路由:`/core/Api/TmTrades/soldget`
 
 参数：
```sh
    input: 
            {
                fields					 必选，需要返回的字段列表，多个字段用半角逗号分隔，可选值为返回示例中能看到的所有字段						
                start_created	         查询三个月内交易创建时间开始。格式:yyyy-MM-dd HH:mm:ss													
                end_created	             查询交易创建时间结束。格式:yyyy-MM-dd HH:mm:ss															
                status			         交易状态																									
                buyer_nick		         买家昵称																									
                type			         交易类型列表																								
                ext_type		         扩展类型																									
                rate_status	             评价状态 																									
                tag			             卖家对交易的自定义分组标签																				
                page_no		             页码																										
                page_size		         每页条数																									
                use_has_next	         是否启用has_next的分页方式，如果指定true,则返回的结果中不包含总记录数，但是会新增一个是否存在下一页的的字段
            }
    output:
            s: 1 || error_code
            d:[     ]
            m: 
    
```