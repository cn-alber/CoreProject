import {
  injectReducers
} from 'store/reducers'

export default (store) => ({
  //path: '',
  getComponent(nextState, cb) {
    require.ensure([], (require) => {
      const Container = require('./containers/DashBordContainer').default
      const reducers = require('./modules/reducers').default
      injectReducers(store, reducers)
      cb(null, Container)
    }, 'dashbord')
  }
})
