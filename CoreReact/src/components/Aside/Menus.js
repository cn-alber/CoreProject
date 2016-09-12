import React from 'react'
import {connect} from 'react-redux'
import styles from './Aside.scss'
import { Menu, Icon } from 'antd'
import {Icon as Iconfa} from 'components/Icon'
import store from 'utils/store' //吃相不太好看
import {changeBookmark} from 'containers/modules/actions'
const SubMenu = Menu.SubMenu
const MenuItemGroup = Menu.ItemGroup

const KEY_OPENKEYS = 'A.menu.OKs'
//const KEY_SELECTED = 'A.menu.sel'
const menuCache = {
  openKeys: store.get(KEY_OPENKEYS, [])
  //selected: store.get(KEY_SELECTED, null)
}
const filterMenus = (menus, filterName) => {
  const filters = []
  let menu
  for (let i = 0; i < menus.length; i++) {
    menu = menus[i]
    if (!menu.type) {
      if (menu.name.indexOf(filterName) !== -1) {
        filters.push(menu)
      }
    } else {
      if (menu.data && menu.data.length) {
        const _filters = filterMenus(menu.data, filterName)
        if (_filters.length) {
          filters.push(Object.assign({}, menu, {
            data: _filters
          }))
        }
      }
    }
  }
  return filters
}
const parseMenu = (menu) => {
  let icon
  switch (typeof menu.icon) {
    case 'string': {
      icon = <Icon type={menu.icon} />
      break
    }
    case 'object': {
      switch (menu.icon[1]) {
        case 'fa': {
          icon = <Iconfa type={menu.icon[0]} />
          break
        }
        default: {
          icon = <Icon type={menu.icon[0]} />
          break
        }
      }
      break
    }
    default: {
      icon = null
      break
    }
  }
  switch (menu.type) {
    case 1: {
      return (
        <MenuItemGroup key={menu.id} title={<span>{icon}&nbsp;&nbsp;<span>{menu.name}</span></span>}>
          {
            menu.data.map((_menu, i) => parseMenu(_menu))
          }
        </MenuItemGroup>
      )
    }
    case 2: {
      return (
        <SubMenu key={menu.id} title={<span>{icon}<span>{menu.name}</span></span>}>
          {
            menu.data.map((_menu, i) => parseMenu(_menu))
          }
        </SubMenu>
      )
    }
    default: {
      const key = menu.id + '_' + menu.path
      return (
        <Menu.Item key={key} path={menu}>
          {icon}{menu.name}
        </Menu.Item>
      )
    }
  }
}

const Menus = React.createClass({
  // _menuSelect({ key }) { //item, key, selectedKeys
  //   console.log('selected', key)
  //   this.props.dispatch({ type: 'BOOKMARKIDS_UPDATE', update: {
  //     $push: [key]
  //   }})
  //   //store.set(KEY_SELECTED, menuCache.selected)
  // },
  _menuClick(e) {
    //console.log('_menuClick ', e.item.props, e)
    this.props.dispatch(changeBookmark(e.item.props.path, true))
  },
  _menuOpen({ key }) { //key, item, keyPath
    menuCache.openKeys.push(key)
    store.set(KEY_OPENKEYS, menuCache.openKeys)
  },
  _menuClose({ key }) { //key, item, keyPath
    menuCache.openKeys = menuCache.openKeys.filter(item => item !== key)
    store.set(KEY_OPENKEYS, menuCache.openKeys)
  },
  renderMenus() {
    const {permissionMenus, permissionMenuFilterName} = this.props
    if (!permissionMenuFilterName) {
      return permissionMenus.length ? permissionMenus.map((menu, i) => parseMenu(menu, i)) : (
        <Menu.Item key='50324' path='' disabled>
          没有权限菜单
        </Menu.Item>
      )
    }
    const permissionMenusFilter = filterMenus(permissionMenus, permissionMenuFilterName)
    return permissionMenusFilter.length ? permissionMenusFilter.map((menu, i) => parseMenu(menu, i)) : (
      <Menu.Item key='159959' path='' disabled>
        没有相关菜单
      </Menu.Item>
    )
  },
  render() {
    const mode = 'inline'
    const {openKeys} = menuCache
    //console.log('menu render')
    return (
      <Menu theme='dark'
        onClick={this._menuClick}
        onOpen={this._menuOpen}
        onClose={this._menuClose}
        className={styles.menus}
        defaultOpenKeys={openKeys}
        mode={mode}
      >
      {this.renderMenus()}
      </Menu>
    )
  }
})
export default connect(state => ({
  permissionMenus: state.permissionMenus,
  permissionMenuFilterName: state.permissionMenuFilterName
}))(Menus)
