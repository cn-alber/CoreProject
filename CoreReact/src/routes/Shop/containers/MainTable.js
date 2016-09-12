import React from 'react'
import {connect} from 'react-redux'
import { Table, Button, message } from 'antd'
import {Request} from 'utils/Xfetch'
import styles from 'components/App.scss'

import json2csv from 'json2csv'
import {saveAs} from 'file-saver'

const moment = window.moment

const tableColumns = [
  {
    title: ' ',
    key: '__',
    render: (a) => a.indexForSort + 1
  }, {
    title: '日期',
    dataIndex: 'io_date',
    key: 'io_date'
  }, {
    title: '商店',
    dataIndex: 'shop_name',
    key: 'shop_name'
  }, {
    title: '分销商',
    dataIndex: 'buyer_shop_name',
    key: 'buyer_shop_name'
  }, {
    title: '销售金额',
    dataIndex: 'saleamount',
    key: 'saleamount',
    sorter: (a, b) => a.saleamount - b.saleamount
  }, {
    title: '运费收入',
    dataIndex: 'salefreight',
    key: 'salefreight',
    sorter: (a, b) => a.salefreight - b.salefreight
  }, {
    title: '销售单数',
    dataIndex: 'saleqty',
    key: 'saleqty',
    sorter: (a, b) => a.saleqty - b.saleqty
  }
]

class MainTable extends React.Component {

  constructor(props) {
    super(props)
    this.state = {
      exportCVSLoading: false
    }
  }
  componentWillMount = () => {
  }
  componentDidMount = () => {
    // setTimeout(()=>{
    //   this.exportCSV()
    // },2000)
  }
  tableCN = () => styles.tableRow
  exportCSV = () => {
    this.setState({
      exportCVSLoading: true
    })
    let isFileSaverSupported
    try {
      isFileSaverSupported = !!new Blob()
    } catch (e) {
      isFileSaverSupported = false
    }
    if (isFileSaverSupported) {
      if (!this.props.proxyData) {
        message.error('尚未读取到数据，请先尝试生成报表')
        this.setState({
          exportCVSLoading: false
        })
        return
      }
      const data = this.props.proxyData.tb_datas
      const fields = [
        'io_date', 'shop_name', 'buyer_shop_name', 'saleamount', 'salefreight', 'saleqty'
      ]
      const fieldNames = [
        '报告日期', '商店名', '分销商', '销售金额', '运费收入', '销售单数'
      ]
      json2csv({ data, fields, fieldNames }, (err, csv) => {
        if (err) {
          message.error(err)
        }
        const blob = new Blob([csv], { type: 'text/plain;charset=utf-8' })
        const date = moment().format('YYMMDDHHmm')
        const filename = `report_shop_${date}.csv`
        saveAs(blob, filename)
        this.setState({
          exportCVSLoading: false
        })
      })
      return
    }
    //这里执行数据导出
    Request.get('shop/export', { type: 3, format: 'cvs' }, (s, d, m) => {
    }).then(() => {
      this.setState({
        exportCVSLoading: false
      })
    })
  }

  render() {
    console.log(' -- component {MainTable} render...')
    const {proxyData} = this.props
    return (
      <div>
        <div className={styles.tableCSV}>
          <Button type='ghost' size='small' onClick={this.exportCSV} loading={this.state.exportCVSLoading}>导出CSV</Button>
        </div>
        <div className='mt5'>
          <Table columns={tableColumns} dataSource={proxyData.tb_datas} pagination={false} rowClassName={this.tableCN} />
        </div>
      </div>
    )
  }
}

export default connect(state => ({
  proxyData: state.proxyData
}))(MainTable)
