//import store from 'utils/store'
import {Request} from 'utils/Xfetch'
import {startLoading, endLoading} from 'utils'

export function loginAsync(user, pass, vcode) {
  return (dispatch, getState) => {
    const data = {
      user,
      pass,
      vcode
    }
    startLoading()
    return Request.get('sign/in', data, (s, d, m) => {
      dispatch({ type: 'AUTHED_SET', payload: true })
    }, () => {
      //dispatch({ type: 'AUTHED_SET', payload: false })
    }).then(endLoading)
  }
}
