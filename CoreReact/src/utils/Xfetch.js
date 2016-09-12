import 'whatwg-fetch'
import { message } from 'antd'
import {API_HOST_DEV, API_HOST_PRO} from 'constants/config'

function build_query(obj, num_prefix, temp_key) {
  return obj ? Object.keys(obj).sort().map((key) => {
    const val = obj[key]
    if (Array.isArray(val)) {
      return val.sort().map((val2) => {
        return encodeURIComponent(key) + '=' + encodeURIComponent(val2)
      }).join('&')
    }
    return encodeURIComponent(key) + '=' + encodeURIComponent(val)
  }).join('&') : ''
}
function checkStatus(response) {
  if (response.status >= 200 && response.status < 300) {
    return response
  }
  const error = new Error(response.statusText)
  error.response = response
  throw error
}
function parseJSON(res) {
  return res.json()
}
function random() {
  return ((new Date()).getTime() + Math.floor(Math.random() * 9999))
}

const defaultOptions = {
  headers: {
    Accept: 'application/json',
    //'Service': uri, //后续放这里
    'Content-Type': 'application/json'
  },
  mode: 'cors',
  credentials: 'include' //Use the include value to send cookies in a cross-origin resource sharing (CORS) request.
}

const _get = (uri, params, successCB, errorCB, isDEV) => {
  let data = params
  const API_HOST = isDEV ? API_HOST_DEV : API_HOST_PRO
  let url = API_HOST + uri
  if (params === null) {
    data = { _: random() }
  } else {
    data._ = random()
  }
  url += isDEV ? '&' : '?'
  url += build_query(data)
  const options = Object.assign({}, defaultOptions, {
    method: 'get'
  })
  return fetch(url, options)
  .then(checkStatus)
  .then(parseJSON)
  .then((d) => {
    if (d.s > 0) {
      d.m && message.success(d.m)
      successCB && successCB(d.s, d.d, d.m)
    } else {
      message.error(d.m, 8)
      errorCB && errorCB(d.m, d.s, d.d)
    }
  }).catch((error) => {
    message.error(error.toString(), 8)
    errorCB && errorCB(error, -10086)
  })
}

const _post = (uri, params, successCB, errorCB, isDEV) => {
  let data = params
  const options = Object.assign({}, defaultOptions, {
    method: 'post',
    body: JSON.stringify(data) //build_query(data)后端就可以_POST获取
  })
  const API_HOST = isDEV ? API_HOST_DEV : API_HOST_PRO
  return fetch(API_HOST + uri, options)
  .then(checkStatus)
  .then(parseJSON)
  .then((d) => {
    if (d.s > 0) {
      d.m && message.success(d.m)
      successCB && successCB(d.s, d.d, d.m)
    } else {
      message.error(d.m, 8)
      errorCB && errorCB(d.m, d.s, d.d)
    }
  }).catch((error) => {
    message.error(error.toString(), 8)
    errorCB && errorCB(error, -10086)
  })
}

module.exports = {
  ZGet: function get(uri, params, successCB, errorCB, isDEV) {
    if (typeof params === 'function') {
      return _get(uri, null, params, successCB, errorCB)
    }
    return _get(uri, params, successCB, errorCB, isDEV)
  },
  ZPost: function get(uri, params, successCB, errorCB, isDEV) {
    if (typeof params === 'function') {
      return _post(uri, null, params, successCB, errorCB)
    }
    return _post(uri, params, successCB, errorCB, isDEV)
  }
}
