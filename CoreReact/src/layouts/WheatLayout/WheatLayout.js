import React from 'react'
import {connect} from 'react-redux'
import {message} from 'antd'
import classNames from 'classnames'
import Header from 'components/Header'
import Aside from 'components/Aside'
//import styles from 'styles/layout.scss'
import styles from './WheatLayout.scss'
import 'styles/core.scss'
//import { Scrollbars } from 'react-custom-scrollbars'
import {startLoading, endLoading} from 'utils'
import {ZGet} from 'utils/Xfetch'
import PageLock from 'components/Lock'

const WheatLayout = React.createClass({
  contextTypes: {
    router: React.PropTypes.object
  },

//fuck this
  // childContextTypes: {
  //   location: React.PropTypes.object
  // },
  // getChildContext() {
  //   return { location: this.props.location }
  // },
//fuck end
  componentWillMount() { //this for refreh example: F5
    startLoading()
    return ZGet('profile/refresh', (s, d, m) => {
      this.props.dispatch({ 'type': 'ENTERING_SET', payload: false })
      if (d.isLocked && this.props.locked) {
        this.props.dispatch({ type: 'LOCKED_SET', payload: true })
      }
      this.props.dispatch({ type: 'PERMISSIONMENUS_SET', payload: d.permissionMenus })
      this.props.dispatch({ type: 'USER_SET', payload: d.user })
    }, (m, s, d) => {
      if (s === -10086) {
        message.error('验证登录失败，请刷新重试', 10)
      } else {
        this.context.router.push('go/login')
      }
    }).then(endLoading)
  },
  // componentWillReceiveProps(nextProps) {
  //   console.dir(this.props)
  //   console.dir(nextProps)
  // },
  render() {
    const {children, collapse, locked, entering, mainFixed} = this.props
    if (entering) {
      return null
    }
    if (locked) {
      return (
        <PageLock />
      )
    }
    const CN = classNames(styles.ZHBody, {
      collapse
    }, {
      mainFixed
    })
    return (
      <div className={styles.bbqZH}>
        <div className={CN}>
          <aside className={styles.ZHAside}>
            <Aside />
          </aside>
          <div className={styles.ZHMain}>
            <div className={styles.ZHMainBox}>
              <div className={styles.ZHMainHeader}>
                <Header />
              </div>
              <div className={styles.ZHMainInner}>
                {children}
              </div>
            </div>
          </div>
        </div>
      </div>
    )
  }
})
export default connect(state => ({
  locked: state.locked,
  collapse: state.collapse,
  mainFixed: state.mainFixed,
  entering: state.entering
}))(WheatLayout)
