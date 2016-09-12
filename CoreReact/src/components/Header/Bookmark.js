import React from 'react'
import { findDOMNode } from 'react-dom'
import update from 'react-addons-update'
//import { IndexLink, Link } from 'react-router'
import {Menu} from 'antd'
import {connect} from 'react-redux'
import classNames from 'classnames'
import styles from './Header.scss'
import Scrollbar from 'components/Scrollbars/index'
import {changeBookmark} from 'containers/modules/actions'

//const SubMenu = Menu.SubMenu

const Bookmark = React.createClass({
  // propTypes: {
  //   history: React.PropTypes.object.isRequired
  // },
  // propTypes: {
  //   location: React.PropTypes.object.isRequired
  // },
  contextTypes: {
    router: React.PropTypes.object.isRequired
    //location: React.PropTypes.object //fuck this
  },
  componentDidMount() {
    this.context.router.listen(location => {
      if (location.action === 'POP') { //返回
        //如果已经被清理掉的标签，怎么办
        this.props.dispatch(changeBookmark(location.state))
      }
    })
   //const {collapse} = this.context.store.getState()
    this.menu = findDOMNode(this.refs.contextMenu)
    document.addEventListener('click', this.closeContextMenu)
  },
  componentWillUnmount() {
    document.removeEventListener('click', this.closeContextMenu)
  },
  closeContextMenu() {
    this.menu.style.cssText = 'display: none;'
  },
  handleContextMenu(e) {
    e.preventDefault()
    const index = e.target.getAttribute('data-index') * 1
    let xOffset = Math.max(document.documentElement.scrollLeft, document.body.scrollLeft)
    let yOffset = Math.max(document.documentElement.scrollTop, document.body.scrollTop)
    const rect = findDOMNode(this.refs.main).getBoundingClientRect()
    //console.log(rect)
    //console.log(menu.style)
    this.menu.target_index = index
    const item = this.props.bookmarks[index]
    if (item && item.hold) { //fuck antd!
      this.menu.childNodes[0].style.display = 'none'
    } else {
      this.menu.childNodes[0].style.display = 'block'
    }
    this.menu.style.cssText =
      'left: ' + (e.clientX + xOffset - rect.left) + 'px;' +
      'top: ' + (e.clientY + yOffset - rect.top) + 'px;' +
      'display: block;'
  },
  _activeTab(index, state) {
    this.props.dispatch({ type: 'BOOKMARKAINDEX_SET', payload: index })
    this.context.router.push({
      pathname: '/' + state.path,
      state: state
    })
  },
  _getIndexByBM(bookmark, bookmarks) {
    let index = -1
    bookmarks.every((item, i) => {
      if (item.path === bookmark.path && item.id === bookmark.id) {
        index = i
        return false
      }
      return true
    })
    return index > -1 ? index : 0
  },
  handleMenuClick(e) {
    let index = this.menu.target_index
    let {bookmarks, activeIndex} = this.props
    switch (e.key) {
      case '-1': {
        if (index === 0) {
          return false
        }
        const activeBookmark = bookmarks[activeIndex]
        bookmarks = update(bookmarks, { $splice: [[index, 1]] })
        this.props.dispatch({ type: 'BOOKMARKS_SET', payload: bookmarks })
        if (index !== activeIndex) {
          index = this._getIndexByBM(activeBookmark, bookmarks)
          this._activeTab(index, activeBookmark)
        } else {
          this._activeTab(0, this.props.bookmarks[0])
        }
        break
      }
      case '-11': {
        //关闭除了自己和首页
        const activeBookmark = bookmarks[index]
        const payload = bookmarks.filter((c, i) => c.hold || i === index)
        this.props.dispatch({ type: 'BOOKMARKS_SET', payload })
        if (activeIndex === index) {
          return
        }
        const newIndex = payload.indexOf(activeBookmark)
        if (newIndex > -1) {
          this._activeTab(newIndex, activeBookmark)
        }
        break
      }
      case '-111': {
        const payload = bookmarks.filter((c, i) => c.hold)
        this.props.dispatch({ type: 'BOOKMARKS_SET', payload })
        if (activeIndex === 0) {
          return
        }
        this._activeTab(0, payload[0])
        break
      }
      default:break
    }
  },
  handleChunClick() {
    this.menu.target_index = this.props.activeIndex
    this.handleMenuClick({
      key: '-1'
    })
  },
  handleHuaClick(e) {
    const index = e.target.getAttribute('data-index') * 1
    if (index === this.props.activeIndex) {
      return false
    }
    const item = this.props.bookmarks[index]
    this._activeTab(index, item)
  },
  huaRender(item, index) {
    const CN = classNames(styles.hua, {
      [`${styles.active}`]: this.props.activeIndex === index
    })
    const key = item.id + '_' + item.path
    return (
      <div key={key} className={CN}>
        <span data-index={index} onClick={this.handleHuaClick} onDoubleClick={this.handleChunClick} onContextMenu={this.handleContextMenu}>{item.name}</span>
      </div>
    )
  },
  render() {
    const {bookmarks} = this.props
    console.log('--render')
    return (
      <div className={styles.bookmarks} ref='main'>
        <div className={styles.inner} ref='inner'>
          <Scrollbar autoHide>
            {bookmarks.length && bookmarks.map((item, index) => this.huaRender(item, index))}
          </Scrollbar>
        </div>
        <Menu ref='contextMenu' onClick={this.handleMenuClick} className={styles.contextMenu}>
          <Menu.Item key='-1'>关闭标签</Menu.Item>
          <Menu.Item key='-11'>关闭其它全部</Menu.Item>
          <Menu.Item key='-111'>关闭全部标签</Menu.Item>
        </Menu>
      </div>
    )
  }
})

export default connect(state => ({
  bookmarks: state.bookmarks,
  activeIndex: state.bookmarkAIndex
}))(Bookmark)
