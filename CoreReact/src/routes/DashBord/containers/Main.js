import React from 'react'
//import classNames from 'classnames'
import {connect} from 'react-redux'
import {Row, Col, Icon, Button} from 'antd'
import styles from 'components/App.scss'

import MainTable from './MainTable'

import {saveConfigA, loadData} from '../modules/actions'
import { animationEnd } from 'utils'


class Main extends React.Component {
  //componentDidMount = () => {}
  componentDidUpdate = (prevProps, prevStates) => {
  }
  componentWillUnmount = () => {
  }

  collapseChange = () => {
    this.props.dispatch({
      type: 'SHOPCOLLAPSEA_REVER'
    })
    this.props.dispatch(saveConfigA())
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
        </div>
        <div className={styles.inner}>
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
