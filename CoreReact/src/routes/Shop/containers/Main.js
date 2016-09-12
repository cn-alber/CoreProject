import React from 'react'
//import classNames from 'classnames'
import {connect} from 'react-redux'
import {Row, Col, Icon, Button} from 'antd'
import styles from 'components/App.scss'

import MainTable from './MainTable'

import {saveConfigA, loadData} from '../modules/actions'
import { animationEnd } from 'utils'

import echarts from 'echarts'
// import echarts from 'echarts/lib/echarts'
// import 'echarts/lib/chart/bar'
// import 'echarts/lib/chart/pie'
//const echarts = window.echarts
// const defaultOption = {
//   toolbox: {
//     feature: {
//       // myCSV: {
//       //   show: true,
//       //   title: '导出下载cvs',
//       //   icon: 'M17.5,17.3H33 M17.5,17.3H33 M45.4,29.5h-28 M11.5,2v56H51V14.8L38.4,2H11.5z M38.4,2.2v12.7H51 M45.4,41.7h-28',
//       //   onclick: (ecModel, api, type) => {
//       //     myChart.showLoading()
//       //   },
//       // },
//       // myReload: {
//       //   show: true,
//       //   title: '重载数据',
//       //   icon: 'M3.8,33.4 M47,18.9h9.8V8.7 M56.3,20.1 C52.1,9,40.5,0.6,26.8,2.1C12.6,3.7,1.6,16.2,2.1,30.6 M13,41.1H3.1v10.2 M3.7,39.9c4.2,11.1,15.8,19.5,29.5,18 c14.2-1.6,25.2-14.1,24.7-28.5',
//       //   onclick: (ecModel, api, type) => {
//       //     myChart.showLoading()
//       //   },
//       // },
//       saveAsImage: {}
//     }
//   }
// }
const DefECOption = {
  title: {
    //text: '',
    x: 'center',
    textStyle: {
      color: '#666',
      fontWeight: 'normal',
      fontSize: 15
    }
  },
  tooltip: {
    trigger: 'item',
    formatter: '{b} : {c} ({d}%)',
    extraCssText: 'pointer-events: none;'
  },
  toolbox: {
    feature: {
      saveAsImage: {}
    }
  },
  // legend: {
  //   orient: 'vertical',
  //   left: 'left',
  //   data: []
  // },
  series: [{
    type: 'pie',
    radius: '75%',
    center: ['50%', '52%'],
    data: [],
    selectedMode: 'multiple',
    selectedOffset: 10,
    clockwise: true,
    label: {
      normal: {
        show: false
      },
      emphasis: {
        show: true
      }
    },
    labelLine: {
      normal: {
        smooth: 0.2,
        length: 10,
        length2: 20
      }
    },
    itemStyle: {
      emphasis: {
        shadowBlur: 10,
        shadowOffsetX: 0,
        shadowColor: 'rgba(0, 0, 0, 0.5)'
      }
    }
  }]
}
const DefEC3Option = {
  title: {
    //text: '近' + dateTotal + '天销售',
    //subtext: dateTotal > 0 ? "20" + dateArr[0] + '至' + "20" + dateArr[dateTotal - 1] : '',
    x: 'center',
    textStyle: {
      color: '#666',
      fontWeight: 'normal',
      fontSize: 16
    }
  },
  tooltip: {
    trigger: 'axis',
    extraCssText: 'pointer-events: none;',
    axisPointer: {
      type: false
    }
  },
  toolbox: {
    feature: {
      saveAsImage: {}
    }
  },
  legend: {
    data: ['金额', '订单量'],
    x: 'left'
  },
  grid: {
    left: '3%',
    right: '4%',
    bottom: '3%',
    containLabel: true
  },
  xAxis: [
    {
      type: 'category',
      data: [],
      axisTick: {
        alignWithLabel: true
      }
    }
  ],
  yAxis: [
    {
      name: '金额(元)',
      nameLocation: 'middle',
      type: 'value',
      nameGap: -20,
      nameTextStyle: {
        color: '#058DC7',
        fontSize: 14
      },
      splitLine: {
        lineStyle: {
          color: '#058DC7',
          opacity: 0.38
        }
      }
    },
    {
      name: '订单量(个)',
      nameLocation: 'middle',
      type: 'value',
      nameGap: -20,
      nameTextStyle: {
        color: '#50B432',
        fontSize: 14
      },
      splitLine: {
        lineStyle: {
          color: '#50B432',
          type: 'dashed',
          opacity: 0.38
        }
      }
    }
  ],
  series: [
    {
      name: '金额',
      type: 'bar',
      barWidth: '25%',
      yAxisIndex: 0,
      silent: true,
      itemStyle: {
        normal: {
          color: '#058DC7'
        }
      },
      data: []
    },
    {
      name: '订单量',
      type: 'line',
      yAxisIndex: 1,
      symbol: 'circle',
      symbolSize: 10,
      showAllSymbol: true,
      smooth: true,
      silent: true,
      z: 3,
      itemStyle: {
        normal: {
          color: '#50B432'
        }
      },
      lineStyle: {
        normal: {
          type: 'dashed',
          width: 1
        }
      },
      data: []
    }
  ]
}

class Main extends React.Component {
  //componentDidMount = () => {}
  componentDidUpdate = (prevProps, prevStates) => {
    if (typeof this.EChartInited === 'undefined') {
      this.EChartInited = 1
      if (this.props.collapse !== prevProps.collapse) {
        //animationEnd(this.refs.main, this.initECharts) //didmount已经开始执行了，so
        this.initEChartTimer = setTimeout(this.initECharts, 800)
      } else {
        this.initECharts()
      }
    } else if (this.props.collapse === prevProps.collapse) { //因为只有collapse和proxyData
      if (this.EChartInited !== 1) {
        this.updateECharts()
      }
    }
  }
  componentWillUnmount = () => {
    clearTimeout(this.initEChartTimer)
    echarts.dispose(this.refs.EC1)
    echarts.dispose(this.refs.EC2)
    echarts.dispose(this.refs.EC3)
    echarts.dispose(this.refs.EC4)
    window.removeEventListener('resize', this.resize, false)
  }

  initECharts = () => {
    this.EChartInited = 2
    this.EC1 = echarts.init(this.refs.EC1)
    this.EC1.setOption(DefECOption)
    this.EC2 = echarts.init(this.refs.EC2)
    this.EC2.setOption(DefECOption)
    this.EC3 = echarts.init(this.refs.EC3)
    this.EC3.setOption(DefEC3Option)
    this.EC4 = echarts.init(this.refs.EC4)
    this.EC4.setOption(DefECOption)
    this.updateECharts()
    window.addEventListener('resize', this.resize, false)
    animationEnd(this.refs.main, this.resize)
  }
  updateECharts = () => {
    const proxyData = this.props.proxyData
    //(() => {
    const sum_amount = []
    const sum_qyts = []
    const sum_freights = []
    if (proxyData.sum_datas && proxyData.sum_datas.length) {
      for (let i = 0; i < proxyData.sum_datas.length; i++) {
        const _data = proxyData.sum_datas[i]
        sum_amount.push({
          value: _data.saleamount,
          name: _data.shop_name
        })
        sum_qyts.push({
          value: _data.saleqty,
          name: _data.shop_name
        })
        sum_freights.push({
          value: _data.salefreight,
          name: _data.shop_name
        })
      }
    }
    this.EC1.setOption({
      title: {
        text: '店铺金额'
      },
      series: [{
        data: sum_amount.sort((a, b) => {
          return a.value > b.value
        })
      }]
    })
    this.EC2.setOption({
      title: {
        text: '店铺单量'
      },
      series: [{
        data: sum_qyts.sort((a, b) => {
          return a.value > b.value
        })
      }]
    })
    this.EC4.setOption({
      title: {
        text: '店铺运费'
      },
      series: [{
        data: sum_freights.sort((a, b) => {
          return a.value > b.value
        })
      }]
    });
    //})()

    (() => {
      let dateLen = 0
      const dates = []
      const amounts = []
      const qtys = []
      if (proxyData.ec_datas) {
        for (let dt in proxyData.ec_datas) {
          dates.push(dt)
          amounts.push(proxyData.ec_datas[dt].amount)
          qtys.push(proxyData.ec_datas[dt].qty)
        }
        dateLen = dates.length
      }
      this.EC3.setOption({
        title: {
          subtext: dateLen > 0 ? dates[0] + '至' + dates[dateLen - 1] : ''
        },
        xAxis: [
          {
            data: dates
          }
        ],
        series: [
          {
            data: amounts
          },
          {
            data: qtys
          }
        ]
      })
    })()
  }
  resize = () => {
    this.EC1.resize()
    this.EC2.resize()
    this.EC3.resize()
    this.EC4.resize()
  }

  collapseChange = () => {
    this.props.dispatch({
      type: 'SHOPCOLLAPSEA_REVER'
    })
    this.props.dispatch(saveConfigA())
  }
  reloadAll = () => {
    this.props.dispatch(loadData())
  }

  render() {
    console.log(' -- component {Main} render...')
    const collapse = this.props.collapse

    //{tableFlag && this.cache.table}
    return (
      <div className={styles.main} ref='main'>
        <div className={styles.title}>
          {collapse
            ? <Button type='ghost' size='small' shape='circle' icon='right' onClick={this.collapseChange} />
            : <Button type='dashed' size='small' shape='circle' icon='left' onClick={this.collapseChange} />}
          &emsp;<Button type='primary' size='small' onClick={this.reloadAll}>生成报表</Button>
        </div>
        <div className={styles.inner}>
          <Row>
            <Col span={7}>
              <div className={styles.ecBox}>
                <div className={styles.head}>
                  <Icon type='reload' />
                </div>
                <div ref='EC1' className={styles.ecDiv}>
                  loading...
                </div>
              </div>
            </Col>
            <Col span={7}>
              <div className={styles.ecBox}>
                <div className={styles.head}>
                  <Icon type='reload' />
                </div>
                <div className={styles.ecDiv} ref='EC2'>
                  loading...
                </div>
              </div>
            </Col>
            <Col span={7}>
              <div className={styles.ecBox}>
                <div className={styles.head}>
                  <Icon type='reload' />
                </div>
                <div className={styles.ecDiv} ref='EC4'>
                  loading...
                </div>
              </div>
            </Col>
          </Row>
          <div className='clearfix' />
          <div className={styles.ecBox}>
            <div className={styles.head}>
              <Icon type='reload' />
            </div>
            <div className={styles.ecDiv} ref='EC3'>
              loading...
            </div>
          </div>
          <MainTable />
        </div>
      </div>
    )
  }
}

export default connect(state => ({
  collapse: state.ShopCollapseA,
  proxyData: state.proxyData
}))(Main)
