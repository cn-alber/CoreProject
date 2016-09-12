import {
  injectReducers
} from 'store/reducers'

export default (store) => ({
  path: 'login',
  getComponent(nextState, cb) {
    require.ensure([], (require) => {
      const Container = require('./containers/LoginContainer').default
      const reducers = require('./modules/reducers').default
      injectReducers(store, reducers)
      cb(null, Container)
    }, 'login')
  }
})
