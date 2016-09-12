import React from 'react'
import {connect} from 'react-redux'
import { Icon, Button, Tooltip, TreeSelect, Checkbox, Collapse, Select, DatePicker, TimePicker } from 'antd'
import constants from 'constants'
import styles from 'components/App.scss'
import store from 'utils/store'
import {saveConfigA} from '../modules/actions'

const CheckboxGroup = Checkbox.Group
const Panel = Collapse.Panel
const Option = Select.Option
const moment = window.moment
const {StoreKeyA: StoreKey} = constants

class Side extends React.Component {
  // constructor(props) {
  //   super(props);
  //   this.state = {
  //     config: {
  //       collapse: false,
  //       autoSave: false,
  //       collapseActives: ['1'],
  //       orderStates: [],
  //       storeActives: [],
  //       traderActives: [],
  //       dateSel: '1',
  //       dateType: '1',
  //       dateCache: {
  //         ['1']: {
  //           from: '00:00:00',
  //           to: moment().format('HH:mm:ss'),
  //         },
  //       },
  //     },
  //   }
  // }
  componentWillMount = () => {
  }
  componentDidMount = () => {
  }
  componentWillUnmount = () => {
  }

  _setState = (obj) => {
    this.props.dispatch({ type: 'SHOPCONFIGA_UPDATE', update: obj })
    this.props.dispatch(saveConfigA())
    // this.setState(update(this.state, {
    //   config: obj,
    // }), () => {
    //   if (this.state.config.autoSave) {
    //     store.set(StoreKey, this.state.config);
    //   }
    //   if (typeof cb === 'function') {
    //     cb();
    //   }
    // });
  }
  autoSaveChange = () => {
    const autoSave = !this.props.config.autoSave
    this.props.dispatch({ type: 'SHOPCONFIGA_UPDATE', update: {
      autoSave: {
        $set: autoSave
      }
    } })
    if (!autoSave) {
      store.remove(StoreKey)
    } else {
      store.set(StoreKey, { ...this.props.config, autoSave })
    }
  }

  selectStores = (value) => {
    this._setState({
      storeActives: {
        $set: value
      }
    })
  }
  selectTraders = (value) => {
    this._setState({
      traderActives: {
        $set: value
      }
    })
  }

  //自动分组
  doGroupData = (_treeData) => {
    const len = _treeData.length
    const i_max = 10
    const j_max = Math.ceil(len / i_max)
    const treeData = []
    for (let j = 0; j < j_max; j++) {
      const obj = {
        label: `自动分组${j + 1}`,
        value: `-${j}`,
        key: `-${j}`,
        children: []
      }
      const max = j + 1 === j_max ? len - i_max * j : i_max
      for (let i = 0; i < max; i++) {
        const index = j * i_max + i
        const ch = _treeData[index]
        obj.children.push({
          label: ch.name,
          value: `${ch.id}`,
          key: `${ch.id}`
        })
      }
      treeData.push(obj)
    }
    return treeData
  }
  orderStateChange = (value) => {
    this._setState({
      orderStates: {
        $set: value
      }
    })
  }
  panelCollapseChange = (value) => {
    this._setState({
      collapseActives: {
        $set: value
      }
    })
  }

  dateSelChange = (value) => {
    const obj = {
      dateSel: {
        $set: value
      }
    }
    switch (value) {
      case '1': {
        if (!this.props.config.dateCache[value]) {
          obj.dateCache = {
            [value]: {
              $set: {
                from: '00:00:00',
                to: moment().format('HH:mm:ss')
              }
            }
          }
        }
        break
      }
      case '2': {
        if (!this.props.config.dateCache[value]) {
          obj.dateCache = {
            [value]: {
              $set: {
                day: moment().format('YYYY-MM-DD')
              }
            }
          }
        }
        break
      }
      case '3': {
        if (!this.props.config.dateCache[value]) {
          obj.dateCache = {
            [value]: {
              $set: {
                month: moment().format('YYYY-MM')
              }
            }
          }
        }
        break
      }
      case '4': {
        if (!this.props.config.dateCache[value]) {
          obj.dateCache = {
            [value]: {
              $set: {
                from: moment().format('YYYY-MM-DD 00:00:00'),
                to: moment().format('YYYY-MM-DD HH:mm:ss')
              }
            }
          }
        }
        break
      }
      default:break
    }
    this._setState(obj)
  }
  dateTypeChange = (value) => {
    this._setState({
      dateType: {
        $set: value
      }
    })
  }
  getDisabledDate = (current) => {
    return current && current.getTime() > Date.now()
  }
  disabledStartDate4 = (startValue) => {
    if (!startValue || !this.props.config.dateCache['4'] || !this.props.config.dateCache['4'].to) {
      return false
    }
    const st = startValue.getTime()
    const mm = moment(this.props.config.dateCache['4'].to)
    return st >= mm.valueOf() || st < mm.subtract(30, 'days').valueOf()
  }
  disabledEndDate4 = (endValue) => {
    if (!endValue || !this.props.config.dateCache['4'] || !this.props.config.dateCache['4'].from) {
      return false
    }
    const et = endValue.getTime()
    const mm = moment(this.props.config.dateCache['4'].from)
    return et <= mm.valueOf() || et > mm.add(30, 'days').valueOf()
  }
  _updateDPC = (obj) => {
    this._setState({
      dateCache: obj
    })
    // this._setState(update(this.state, {
    //   dateCache: obj,
    // }), cb);
  }
  datePickChange1(field, date, dateString) {
    const method = !this.props.config.dateCache['1'] ? '$set' : '$merge'
    this._updateDPC({
      '1': { [`${method}`]: { [`${field}`]: dateString || null } }
    })
  }
  datePickChange2(date, dateString) {
    const method = !this.props.config.dateCache['2'] ? '$set' : '$merge'
    this._updateDPC({
      '2': { [`${method}`]: { day: dateString || null } }
    })
  }
  datePickChange3(date, dateString) {
    const method = !this.props.config.dateCache['3'] ? '$set' : '$merge'
    this._updateDPC({
      '3': { [`${method}`]: { month: dateString || null } }
    })
  }
  datePickChange4(field, date, dateString) {
    const method = !this.props.config.dateCache['4'] ? '$set' : '$merge'
    this._updateDPC({
      '4': { [`${method}`]: { [`${field}`]: dateString || null } }
    })
  }
  datePickerArea = () => {
    const date = this.props.config.dateCache[this.props.config.dateSel] || {}
    switch (this.props.config.dateSel) {
      case '1': { //今日
        const { from, to } = date
        return (
          <div>
            <div><TimePicker placeholder='开始时间' value={from} onChange={this.datePickChange1.bind(this, 'from')} /></div>
            <div className='mt5'><TimePicker placeholder='结束时间' value={to} onChange={this.datePickChange1.bind(this, 'to')} /></div>
          </div>
        )
      }
      case '2': { //自然日
        const { day } = date
        return (
          <div>
            <DatePicker value={day} disabledDate={this.getDisabledDate} onChange={this.datePickChange2.bind(this)} />
          </div>
        )
      }
      case '3': { //自然月
        const { month } = date
        return (
          <div>
            <DatePicker.MonthPicker value={month} disabledDate={this.getDisabledDate} onChange={this.datePickChange3.bind(this)} />
          </div>
        )
      }
      case '4': { //自由
        const { from, to } = date
        return (
          <div>
            <DatePicker disabledDate={this.disabledStartDate4} value={from} showTime format='yyyy-MM-dd HH:mm:ss' placeholder='开始日期' onChange={this.datePickChange4.bind(this, 'from')} />
            <div className='mt5'><DatePicker disabledDate={this.disabledEndDate4} value={to} showTime format='yyyy-MM-dd HH:mm:ss' placeholder='开始日期' onChange={this.datePickChange4.bind(this, 'to')} /></div>
          </div>
        )
      }
      default: return null
    }
  }

  render() {
    console.log(' -- component {Side} render...')
    const { autoSave, collapseActives, orderStates, dateSel, dateType, dateCache } = this.props.config
    const _treeData = []
    for (let i = 0; i < 30; i++) {
      _treeData.push({
        name: `店铺${i}`,
        id: `${i}`
      })
    }
    const treeData = this.doGroupData(_treeData)
    const tPropsStore = {
      treeData,
      value: this.props.config.storeActives,
      onChange: this.selectStores,
      multiple: true,
      treeCheckable: true,
      searchPlaceholder: '请选择店铺',
      style: {
        width: '100%'
      }
    }
    const tPropsTrader = {
      treeData,
      value: this.props.config.traderActives,
      onChange: this.selectTraders,
      multiple: true,
      treeCheckable: true,
      searchPlaceholder: '请选择分销商',
      style: {
        width: '100%'
      }
    }

    // const dateTypes = [
    //   {
    //     value: '1',
    //     label: '发货日期',
    //   },
    //   {
    //     value: '2',
    //     label: '订单日期',
    //   },
    //   {
    //     value: '3',
    //     label: '付款日期',
    //   },
    // ];
    const orderStatesOpts = [
      {
        value: '1',
        label: '已付款待审核'
      },
      {
        value: '2',
        label: '已审核待配快递'
      },
      {
        value: '3',
        label: '发货中(打单拣货)'
      },
      {
        value: '4',
        label: '已发货'
      },
      {
        value: '5',
        label: '等供销商发货'
      }
    ]
    // const { tableFlag } = this.state.proxyData

    return (
      <div className={styles.side}>
        <div className={styles.inner}>
          <div className={styles.sideTop}>
            {autoSave ? <Button type='ghost' size='small' onClick={this.autoSaveChange}>
              <Tooltip title='点击取消自动保存本页配置'><Icon type='lock' /></Tooltip>
            </Button> : <Button type='ghost' size='small' onClick={this.autoSaveChange}>
              <Tooltip title='点击自动保存本页配置到本地'><Icon type='unlock' /></Tooltip>
            </Button>}
          </div>
          <div className='myCollapse'>
            <Collapse activeKey={collapseActives} onChange={this.panelCollapseChange}>
              <Panel header='日期条件' key='1'>
                <div className={styles.box}>
                  <div>
                    <Select size='small' value={dateSel} onChange={this.dateSelChange}>
                      <Option value='1'>今日匹配</Option>
                      <Option value='2'>按天匹配</Option>
                      <Option value='3'>按月匹配</Option>
                      <Option value='4'>自由匹配</Option>
                    </Select>
                    &nbsp;
                    <Select size='small' value={dateType} onChange={this.dateTypeChange}>
                      <Option value='1'>发货日期</Option>
                      <Option value='2'>订单日期</Option>
                      <Option value='3'>付款日期</Option>
                    </Select>
                  </div>
                  <div className={styles.dateSel}>
                    {this.datePickerArea()}
                  </div>
                </div>
              </Panel>
              <Panel header='订单状态' key='2'>
                <div className={styles.box}>
                  <Tooltip placement='left' title='状态：默认全选'><Icon type='question-circle-o' /></Tooltip>
                  <div className='CBGLine'>
                    <CheckboxGroup options={orderStatesOpts} value={orderStates} onChange={this.orderStateChange} />
                  </div>
                </div>
              </Panel>
              <Panel header='店铺:' key='3'>
                <div className={styles.box}>
                  <Tooltip placement='right' title='店铺：默认全选'><Icon type='question-circle-o' /></Tooltip>
                  <TreeSelect {...tPropsStore} />
                </div>
              </Panel>
              <Panel header='分销商:' key='4'>
                <div className={styles.box}>
                  <Tooltip placement='right' title='分销商：默认全选'><Icon type='question-circle-o' /></Tooltip>
                  <TreeSelect {...tPropsTrader} />
                </div>
              </Panel>
            </Collapse>
          </div>
        </div>
      </div>
    )
  }
}

export default connect(state => ({
  config: state.ShopConfigA
}))(Side)
