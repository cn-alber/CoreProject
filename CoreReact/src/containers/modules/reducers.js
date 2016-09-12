import { handleActions } from 'redux-actions'
import update from 'react-addons-update'
import store from 'utils/store' //吃相不太好看
//import { combineReducers } from 'redux'

const entering = handleActions({
  ENTERING_SET: (state, action) => action.payload
}, true)
const loading = handleActions({
  LOADING_SET: (state, action) => action.payload
}, true)
const locked = handleActions({
  LOCKED_SET: (state, action) => {
    const newState = action.payload
    store.set('g.locked', newState)
    return newState
  }
}, store.get('g.locked', false))

const authed = handleActions({
  AUTHED_SET: (state, action) => action.payload
}, false)
const user = handleActions({
  USER_SET: (state, action) => (action.payload),
  //USER_REVER: (state, action) => !state,
  USER_UPDATE: (state, action) => {
    return update(state, action.update)
  }
}, {})
const permissionMenus = handleActions({
  PERMISSIONMENUS_SET: (state, action) => (action.payload),
  PERMISSIONMENUS_UPDATE: (state, action) => {
    return update(state, action.update)
  }
}, [])
const permissionMenuFilterName = handleActions({
  PERMISSIONMENUFILTERNAME_SET: (state, action) => (action.payload)
}, [])

//页面是否分屏
const mainFixed = handleActions({
  MAINFIXED_SET: (state, action) => action.payload,
  MAINFIXED_REVER: (state, action) => !state
}, false)
const collapse = handleActions({
  COLLAPSE_SET: (state, action) => {
    const newState = action.payload
    store.set('g.collapse', newState)
    return newState
  },
  COLLAPSE_REVER: (state, action) => {
    const newState = !state
    store.set('g.collapse', newState)
    return newState
  }
}, store.get('g.collapse', false))

const proset_visibel = handleActions({
  PROSET_VISIBEL_SET: (state, action) => action.payload,
  PROSET_VISIBEL_REVER: (state, action) => !state
}, false)
const notice_visibel = handleActions({
  NOTICE_VISIBEL_SET: (state, action) => action.payload,
  NOTICE_VISIBEL_REVER: (state, action) => !state
}, false)

const bookmarkAIndex = handleActions({
  BOOKMARKAINDEX_SET: (state, action) => {
    const newState = action.payload
    store.set('g.bookmarkAIndex', newState)
    return newState
  }
}, store.get('g.bookmarkAIndex', 0))
const bookmarks = handleActions({
  BOOKMARKS_SET: (state, action) => {
    const newState = action.payload
    store.set('g.bookmarks', newState)
    return newState
  },
  BOOKMARKS_UPDATE: (state, action) => {
    const newState = update(state, action.update)
    store.set('g.bookmarks', newState)
    return newState
  }
}, store.get('g.bookmarks', [
  {
    name: '首页',
    path: '',
    hold: true,
    id: '0'
  }
]))

// const bookmarkIDs = handleActions({
//   BOOKMARKIDS_SET: (state, action) => (action.payload),
//   BOOKMARKIDS_UPDATE: (state, action) => {
//     return update(state, action.update)
//   }
// }, store.get('g.bookmarkids', []))

export default {
  entering, loading,
  locked, authed, proset_visibel, notice_visibel,
  user, permissionMenus, permissionMenuFilterName,
  collapse, mainFixed,
  bookmarks, bookmarkAIndex
}
