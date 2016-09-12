import { handleActions } from 'redux-actions'
import update from 'react-addons-update'
//import { combineReducers } from 'redux'

const enterLoading = handleActions({
  ENTERLOADING_SET: (state, action) => (action.payload)
}, true)

const proxyData = handleActions({
  PROXYDATA_SET: (state, action) => action.payload
}, {})
const ShopCollapseA = handleActions({
  SHOPCOLLAPSEA_SET: (state, action) => action.payload,
  SHOPCOLLAPSEA_REVER: (state, action) => !state
}, true)
const ShopGolbalA = handleActions({
  SHOPGOLBALA_SET: (state, action) => (action.payload)
}, {})
const ShopConfigA = handleActions({
  SHOPCONFIGA_SET: (state, action) => (action.payload),
  SHOPCONFIGA_MERGE: (state, action) => {
    return update(state, {
      $merge: action.merge
    })
  },
  SHOPCONFIGA_UPDATE: (state, action) => {
    return update(state, action.update)
    // const newState = update(state, action.update);
    // if (action.storeKey && newState.autoSave) {
    //   store.set(action.storeKey, newState);
    // }
    // return newState;
  }
}, {
  //collapse: false,
  autoSave: false,
  collapseActives: ['1'],
  orderStates: [],
  storeActives: [],
  traderActives: [],
  dateSel: '1',
  dateType: '1',
  dateCache: {
    ['1']: {
      from: '00:00:00',
      to: '23:59:59'
    }
  }
})
export default {
  enterLoading,
  ShopCollapseA, ShopGolbalA, ShopConfigA,
  proxyData
}
