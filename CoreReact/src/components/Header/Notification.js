import React from 'react'
import { Modal, Checkbox, Radio, Button } from 'antd'
import {connect} from 'react-redux'
import {ZPost} from 'utils/Xfetch'
import store from 'utils/store' //吃相不太好看
import styles from './Header.scss'
import {AgGridReact} from 'ag-grid-react'
import 'ag-grid-root/dist/styles/ag-grid.css'
import 'styles/ag-grid.scss'
import {ZHCN} from 'constants/gridLocaleText'

const CheckboxGroup = Checkbox.Group
const RadioGroup = Radio.Group

const levels = [
  { label: '严重', value: '3' },
  { label: '重要', value: '2' },
  { label: '普通', value: '1' },
  { label: '不重要', value: '0' }
]

const DEFLEVELS_KEY = 'msg.deflevels'
const DEFREADED_KEY = 'msg.defReaded'
const COLUMNS_KEY = 'msg.columns'
let defLevels = store.get(DEFLEVELS_KEY, ['0', '1', '2', '3'])
let defReaded = store.get(DEFREADED_KEY, '0')
const defColumns = [
  {
    headerName: '#',
    width: 30,
    checkboxSelection: true,
    pinned: true
  }, {
    headerName: '优先级',
    field: 'MsgLevel',
    width: 60,
    pinned: true,
    cellClassRules: {
      'lv-1': (params) => { return params.value === '已发货' },
      'lv-2': (params) => { return params.value === '发货中' },
      'lv-3': (params) => { return params.value === '待付款' }
    }
  }, {
    headerName: '消息',
    field: 'Msg',
    width: 500
  }, {
    headerName: '发送时间',
    field: 'CreateDate',
    width: 120
  }, {
    headerName: '已阅',
    //field: 'CreateDate',
    width: 40
  }, {
    headerName: '阅读人',
    field: 'Reador',
    width: 70
  }, {
    headerName: '阅读时间',
    field: 'ReadDate',
    width: 120
  }]
const defState = store.get(COLUMNS_KEY, null)

const gridOptions = {
  onModelUpdated: () => {
    console.log('event onModelUpdated received')
  },
  rowBuffer: 10, // no need to set this, the default is fine for almost all scenarios
  rowModelType: 'pagination',
  //enableColResize: true,
  localeText: ZHCN,
  paginationPageSize: 20,
  datasource: {
    rowCount: 0,
    getRows: function(params) {
      params.failCallback()
    }
  }
}
const Notification = React.createClass({

  getInitialState() {
    return {
      searchLoading: false
    }
  },
  componentDidMount() {
  },
  componentWillUnmount() {
  },
  hideModal() {
    this.props.dispatch({ type: 'NOTICE_VISIBEL_SET', payload: false })
  },
  handleLevel(checkedValues) {
    defLevels = checkedValues
    store.set(DEFLEVELS_KEY, checkedValues)
  },
  handleReaded(e) {
    defReaded = e.target.value
    store.set(DEFREADED_KEY, defReaded)
  },
  handleSearch() {
    const data = {
      readed: defReaded,
      levels: defLevels
    }
    //这个是用来搜索定义
    this.api.setDatasource({
      rowCount: 200,
      getRows: this.getRows
    })
    // ZPost('profile/msg', data, (s, d, m) => {
    //   console.log(d)
    // })
  },
  getRows(params) {
    console.log(params)
    params.failCallback()
    //params.successCallback(rowData)
  },
  onGridReady(params) {
    this.api = params.api
    this.columnApi = params.columnApi
    if (defState) {
      this.columnApi.setColumnState(defState)
    }
  },
  agColumnResized(e) {
    if (e.finished) {
      store.set(COLUMNS_KEY, this.columnApi.getColumnState())
    }
  },
  agColumnMoved(e) {
    console.log(e)
    store.set(COLUMNS_KEY, this.columnApi.getColumnState())
  },
  render() {
    const {visible} = this.props
    if (!visible && !this._firstVisibled) {
      this._firstVisibled = true
      return null
    }
    const {searchLoading} = this.state
    return (
      <Modal wrapClassName='modalTop' width='100%' title='通知' visible={visible} onCancel={this.hideModal} footer=''>
        <div className='clearfix'>
          <div className={styles.levels}>
            <CheckboxGroup options={levels} defaultValue={defLevels} onChange={this.handleLevel} />
          </div>
          <div className={styles.readed}>
            <RadioGroup onChange={this.handleReaded} defaultValue={defReaded}>
              <Radio key='-1' value=''>全部</Radio>
              <Radio key='1' value={1}>已读</Radio>
              <Radio key='0' value={0}>未读</Radio>
            </RadioGroup>
            <Button type='primary' onClick={this.handleSearch} loading={searchLoading}>检索</Button>
          </div>
        </div>
        <div className='clearfix mt10 mb10'>
          <div className=''>
            <Button type='primary' size='small' className='mr10'>新消息</Button>
            <Button type='ghost' size='small' className='mr10'>批量已读</Button>
            <Button type='ghost' size='small' shape='circle-outline' icon='reload' />
          </div>
        </div>
        <div style={{height: 500}} className='ag-fresh'>
          <AgGridReact
            gridOptions={gridOptions}

            onGridReady={this.onGridReady}
            onColumnResized={this.agColumnResized}
            onColumnMoved={this.agColumnMoved}
            showToolPanel={false}
            quickFilterText={null}

            columnDefs={defColumns}

            rowSelection='multiple'
            enableColResize='true'
            groupHeaders='false'
            rowHeight='32'
            debug='false'
          />
        </div>
      </Modal>
    )
  }
})

export default connect(state => ({
  visible: state.notice_visibel
}))(Notification)
