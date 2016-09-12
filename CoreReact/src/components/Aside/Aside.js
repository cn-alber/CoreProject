import React from 'react'
//import {connect} from 'react-redux'
import styles from './Aside.scss'
//import { Menu, Icon } from 'antd'
import Scrollbar from 'components/Scrollbars/index'
import {SOFTNAME} from 'constants/config'
import SearchInput from './SearchInput'
import Menus from './Menus'

const Asider = React.createClass({
  // contextTypes: {
  //   router: React.PropTypes.object.isRequired
  // },
  // getInitialState() {
  //   return {
  //     openKeysDef: []
  //   }
  // },
  componentWillMount() {
    // this.setState({
    //   openKeysDef: menuCache.openKeys
    // })
  },
  render() {
    //  multiple={true}
    //  onSelect={this._menuSelect}
    //selectedKeys={bookmarkIDs}
    return (
      <div className={styles.zhang}>
        <div className={styles.chun}>
          <a className={styles.brand}>{SOFTNAME}</a>
        </div>
        <div className={styles.hua}>
          <Scrollbar autoHide className={styles['s--wrapper']}>
            <Menus />
          </Scrollbar>
          <div className={styles.searchBox}>
            <div className={styles.searchMenus}>
              <SearchInput placeholder='搜索菜单' />
            </div>
          </div>
        </div>
      </div>
    )
  }
})

export default Asider
// export default connect(state => ({
//   //permissionMenus: state.permissionMenus
// }))(Asider)
