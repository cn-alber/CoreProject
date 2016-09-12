import { injectReducers } from 'store/reducers'

export default (store) => ({
  path: 'shop',
  /*  Async getComponent is only invoked when route matches   */
  getComponent(nextState, cb) {
    /*  Webpack - use 'require.ensure' to create a split point
        and embed an async module loader (jsonp) when bundling   */
    require.ensure([], (require) => {
      const Shop = require('./containers/ShopContainer').default
      const reducers = require('./modules/reducers').default

      //const reducer = require('./modules/counter').default
      //injectReducer(store, { key: 'counter', reducer })
      injectReducers(store, reducers)

      /*  Return getComponent   */
      cb(null, Shop)

    /* Webpack named bundle   */
    }, 'shop')
  }
})
