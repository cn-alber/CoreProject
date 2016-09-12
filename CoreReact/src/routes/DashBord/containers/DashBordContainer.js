import React from 'react'
import {connect} from 'react-redux'
import classNames from 'classnames'
import styles from 'components/App.scss'
import MainWrapper from 'components/MainWrapper'

import {Icon as IconFa} from 'components/Icon'

// import echarts from 'echarts/lib/echarts'
// import 'echarts/lib/chart/bar'
// import 'echarts/lib/chart/pie'
//import echarts from 'echarts'
import Scrollbar from 'components/Scrollbars/index'
  // <Scrollbar style={{ height: '80%', overflow: 'hidden' }}>
  // </Scrollbar>
  // <div style={{height: '20%', overflow: 'hidden'}}>
  //   <h1>封装方法：切换路由/本地缓存数据结构/</h1>
  // </div>
let _cacheData
class DashBordContainer extends React.Component {
  componentWillMount = () => {
    console.log('11111111111111111111111111111 componentWillMount 1')
  }
  componentDidMount = () => {
    console.log('11111111111111111111111111111 componentDidMount 2')
    //this.EC = echarts.init(this.refs.EC)
    //this.EC.setOption(options)
    //console.error(_cacheData)
  }
  componentWillUnmount = () => {
    console.log('11111111111111111111111111111 componentWillUnmount 3')
  }
  refreshDataCallback = () => {
    _cacheData = 'getFirst Data'
    console.warn('getFirst Data')
  }

  render() {
    console.log(' -- app DashBord render...')
    return (
      <div className={styles.content}>
        首页，通知，<br />
      router 返回事件
        <IconFa spin type='spinner' />
      </div>
    )
  }
}

export default connect(state => ({

}))(MainWrapper(DashBordContainer, {withRef: true}))
