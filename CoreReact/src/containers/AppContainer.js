import React, { Component, PropTypes } from 'react'
import { Router } from 'react-router'
import { Provider } from 'react-redux'
import {injectReducers} from 'store/reducers'
const reducers = require('./modules/reducers').default

class AppContainer extends Component {
  static propTypes = {
    history: PropTypes.object.isRequired,
    routes: PropTypes.any.isRequired,
    store: PropTypes.object.isRequired
  }
  componentWillMount() {
    //holy shit, what happended?! will todo sth
    injectReducers(this.props.store, reducers)
  }
  render() {
    const { history, routes, store } = this.props

    return (
      <Provider store={store}>
        <Router history={history} children={routes} />
      </Provider>
    )
  }
}

export default AppContainer
