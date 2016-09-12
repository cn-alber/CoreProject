import store from 'utils/store'
import constants from 'constants'
import {Request} from 'utils/Xfetch'
import {startLoading, endLoading} from 'utils'

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
