import React from 'react'
import {connect} from 'react-redux'
import classNames from 'classnames'
import styles from 'components/App.scss'

import {endLoading} from 'utils'
import store from 'utils/store'
import {Request} from 'utils/Xfetch'
import {loadData} from '../modules/actions'
import constants from 'constants'
import EE from 'utils/EE'
import MainWrapper from 'components/MainWrapper'

import Side from './Side'
import Main from './Main'

let _cacheData

class TestContainer extends React.Component {
  componentWillMount = () => {
  }
  componentDidMount = () => {
  }
  componentWillUnmount = () => {
  }
  refreshDataCallback = () => {
    _cacheData = 'getFirst Data'
    console.warn('getFirst Data')
  }
  render() {
    console.log(' -- app Test render...')
    const collapse = this.props.collapse
    const CN = classNames(styles.content, {
      [`${styles.collapse}`]: collapse
    })
    return (
      <div className={CN}>
        <Side />
        <Main />
      </div>
    )
  }
}

export default connect(state => ({collapse: state.ShopCollapseA}))(MainWrapper(TestContainer, {withRef: true}))
