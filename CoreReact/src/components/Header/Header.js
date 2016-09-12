import React from 'react'
//import { IndexLink, Link } from 'react-router'
import {Row, Col, Dropdown, Icon, Menu} from 'antd'
import {connect} from 'react-redux'
import classNames from 'classnames'
import styles from './Header.scss'
import {toggleCollapse} from 'containers/modules/actions'
import Navs from './Navs'
import ModalProset from './ModalProset'
import SF from './SF'
import NoticeBtn from './NoticeBtn'
import {ZPost} from 'utils/Xfetch'
import {startLoading, endLoading} from 'utils'
import Notification from './Notification'

const Header = React.createClass({
  contextTypes: {
    router: React.PropTypes.object
    //location: React.PropTypes.object
  },

//fuck this
  // childContextTypes: {
  //   location: React.PropTypes.object
  // },
  // getChildContext() {
  //   return { location: this.context.location }
  // },
//fuck end
  toggleCollapse() {
    this.props.dispatch(toggleCollapse())
  },
  handleMenuClick(e) {
    switch (e.key) {
      case '0': {
        this.props.dispatch({ type: 'PROSET_VISIBEL_SET', payload: true })
        break
      }
      case '1': {
        startLoading()
        ZPost('profile/lock', {silence: 1}, (s, d, m) => {
          this.props.dispatch({ type: 'LOCKED_SET', payload: true })
        }).then(endLoading)
        break
      }
      case '-1': {
        ZPost('sign/out', (s, d, m) => {
          this.context.router.push('/go/login')
        })
        break
      }
      default:break
    }
  },
  render() {
    const {collapse, user} = this.props
    const CN = classNames(styles.hamburger, styles['hamburger--arrowalt'], styles['js-hamburger'], {
      [`${styles['is-active']}`]: !collapse
    })
    const {name} = user

    const menu = (
      <Menu onClick={this.handleMenuClick}>
        <Menu.Item key='0'>
          <Icon type='setting' />&nbsp;&nbsp;密码修改
        </Menu.Item>
        <Menu.Item key='1'>
          <Icon type='lock' />&nbsp;&nbsp;锁屏
        </Menu.Item>
        <Menu.Divider />
        <Menu.Item key='-1'><Icon type='logout' />&nbsp;&nbsp;安全登出</Menu.Item>
      </Menu>
    )
    return (
      <div className={styles.wraper}>
        <Notification />
        <div id='ZH-menus' className={styles.menus}>
          <Row>
            <Col span={8}>
              <div className={styles.menuL}>
                <div className={CN} onClick={this.toggleCollapse}>
                  <div className={styles['hamburger-box']}>
                    <div className={styles['hamburger-inner']} />
                  </div>
                </div>
                <a className={styles.menuA}>Fire In</a>
                <a className={styles.menuA}>The Hole</a>
              </div>
            </Col>
            <Col span={16}>
              <div className={styles.menuR}>
                <a className={styles.menuA}>客服</a>
                <div className={styles.user}>
                  <Dropdown overlay={menu} trigger={['click']}>
                    <a className='ant-dropdown-link' href='javascript:void(0)'>
                      {name} <Icon type='down' />
                    </a>
                  </Dropdown>
                  <ModalProset />
                </div>
                <NoticeBtn />
                <SF />
              </div>
            </Col>
          </Row>
        </div>
        <Navs />
      </div>
    )
  }
})

//export default Header
export default connect(state => ({
  collapse: state.collapse,
  user: state.user
}))(Header)
