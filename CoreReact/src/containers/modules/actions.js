import store from 'utils/store'
import constants from 'constants'
import {Request} from 'utils/Xfetch'
import {startLoading, endLoading} from 'utils'
//import { browserHistory } from 'react-router'
import { push } from 'react-router-redux'
//import store from 'utils/store'

export function doEndLoading(cb) {
  return (dispatch, getState) => {
    const {entering} = getState()
    if (entering) {
      dispatch({ 'type': 'ENTERING_SET', payload: false })
    }
    endLoading(cb)
  }
}
export function doStartLoading() {
  return (dispatch, getState) => {
    dispatch({ 'type': 'ENTERING_SET', payload: true })
    startLoading()
  }
}
export function toggleCollapse() {
  return (dispatch, getState) => {
    dispatch({ 'type': 'COLLAPSE_REVER' })
    //console.log(getState().collapse)
  }
}
export function closeBookmarkByIndex() {
  return (dispatch, getState) => {
  }
}
export function closeBookmarkByState() {
  return (dispatch, getState) => {
  }
}
/*
{
  name: '订单列表',
  path: '/bbq',
  hold: true,
  id: '3'
}
*/
//根据path||id，如果存在则切换，不存在则新增
export function activeBookmark(bookmark) {
  return (dispatch, getState) => {
    if (!bookmark) {
      return
    }
    const {bookmarks, bookmarkAIndex} = getState()
    let index = -1
    bookmarks.every((item, i) => {
      if (item.path === bookmark.path && item.id === bookmark.id) {
        index = i
        return false
      }
      return true
    })
    if (index > -1 && bookmarkAIndex !== index) {
      dispatch({ type: 'BOOKMARKAINDEX_SET', payload: index })
    }
  }
}
export function changeBookmark(bookmark, pushed) {
  return (dispatch, getState) => {
    if (!bookmark) {
      dispatch({ type: 'BOOKMARKAINDEX_SET', payload: 0 })
      pushed && dispatch(push(''))
      return
    }
    const {bookmarks, bookmarkAIndex} = getState()
    let index = -1
    bookmarks.every((item, i) => {
      if (item.path === bookmark.path && item.id === bookmark.id) {
        index = i
        return false
      }
      return true
    })
    let flag = false
    if (index === -1) { //新增
      index = bookmarks.length
      bookmarks.push(bookmark)
      dispatch({ type: 'BOOKMARKS_SET', payload: bookmarks })
      flag = true
    } else {
      if (bookmarkAIndex !== index) {
        flag = true
      }
    }
    if (flag) {
      dispatch({ type: 'BOOKMARKAINDEX_SET', payload: index })
    }
    if (pushed) {
      dispatch(push({
        pathname: '/' + bookmark.path,
        state: bookmark
      }))
    }
  }
}

export function loadData() {
  return (dispatch, getState) => {
    const { ShopConfigA } = getState()
    const filter = {
      dateSel: ShopConfigA.dateSel,
      dateType: ShopConfigA.dateType,
      dateSelData: ShopConfigA.dateCache[ShopConfigA.dateSel],
      orders: ShopConfigA.orderStates,
      store_ids: ShopConfigA.storeActives,
      trader_ids: ShopConfigA.traderActives
    }
    startLoading()
    Request.get('shop/loadData', filter, (s, d, m) => {
      dispatch({ type: 'PROXYDATA_SET', payload: d })
      endLoading()
    })
  }
}
export function saveConfigA() {
  return (dispatch, getState) => {
    const { ShopCollapseA, ShopConfigA } = getState()
    if (ShopConfigA.autoSave) {
      store.set(constants.StoreKeyA, { ...ShopConfigA, collapse: ShopCollapseA })
    }
  }
}
