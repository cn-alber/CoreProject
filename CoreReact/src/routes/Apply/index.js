import {
  injectReducers
} from 'store/reducers'

export default (store) => ({
  path: 'apply',
  getComponent(nextState, cb) {
    require.ensure([], (require) => {
      const Container = require('./containers/ApplyContainer').default
      const reducers = require('./modules/reducers').default
      injectReducers(store, reducers)
      cb(null, Container)
    }, 'apply')
  }
})
