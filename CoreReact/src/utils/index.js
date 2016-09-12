

// export function exportCSV(filename, fields, data) {
//   //http://stackoverflow.com/questions/3665115/create-a-file-in-memory-for-user-to-download-not-through-server
//   //用这个来判断是否支持，不支持的话后端推送下载
//   // try {
//   //     var isFileSaverSupported = !!new Blob;
//   // } catch (e) {}
//   json2csv({ data, fields }, (err, csv) => {
//     if (err) console.log(err)
//     const blob = new Blob([csv], { type: 'text/plain;charset=utf-8' })
//     saveAs(blob, 'test.csv')
//   })
// }

export function getUriParam(name) {
  const reg = new RegExp(`(^|&)${name}=([^&]*)(&|$)`)
  const r = window.location.search.substr(1).match(reg)
  return r !== null ? unescape(r[2]) : null
}

const loadingNode = document.getElementById('ant-site-loading')
export function endLoading(cb) {
  setTimeout(() => {
    //loadingNode.parentNode.removeChild(loadingNode)
    loadingNode.style.display = 'none'
    if (cb) {
      cb()
    }
  }, 450)
}
export function startLoading() {
  loadingNode.style.display = 'block'
}

export function utf16to8(str) {
  let out = ''
  let c
  if (typeof str !== 'undefined') {
    const len = str.length
    for (let i = 0; i < len; i++) {
      c = str.charCodeAt(i)
      if ((c >= 0x0001) && (c <= 0x007F)) {
        out += str.charAt(i)
      } else if (c > 0x07FF) {
        out += String.fromCharCode(0xE0 | ((c >> 12) & 0x0F))
        out += String.fromCharCode(0x80 | ((c >> 6) & 0x3F))
        out += String.fromCharCode(0x80 | ((c >> 0) & 0x3F))
      } else {
        out += String.fromCharCode(0xC0 | ((c >> 6) & 0x1F))
        out += String.fromCharCode(0x80 | ((c >> 0) & 0x3F))
      }
    }
  }
  return out
}

//https://www.sitepoint.com/css3-animation-javascript-event-handlers/
//https://raw.githubusercontent.com/arve0/react-animate-on-change/master/index.js
const pfx = ['webkit', 'moz', 'MS', 'o', ''];
export function addFixedEventListener(element, type, callback) {
  for (let p = 0; p < pfx.length; p++) {
    if (!pfx[p]) {
      type = type.toLowerCase();
    }
    element.addEventListener(pfx[p] + type, callback, false);
  }
}
export function removeFixedEventListener(element, type, callback) {
  for (let p = 0; p < pfx.length; p++) {
    if (!pfx[p]) {
      type = type.toLowerCase();
    }
    element.removeEventListener(pfx[p] + type, callback, false);
  }
}
export function animationStart(element, callback, flag) {
  if (flag) {
    removeFixedEventListener(element, 'AnimationStart', callback);
  } else {
    addFixedEventListener(element, 'AnimationStart', callback);
  }
}
export function animationEnd(element, callback, flag) {
  if (flag) {
    removeFixedEventListener(element, 'AnimationEnd', callback);
  } else {
    addFixedEventListener(element, 'AnimationEnd', callback);
  }
}

const whichTransitionEvent = (() => {
  const el = document.createElement('fakeElement');
  const transitions = {
    transition: 'transitionend',
    OTransition: 'oTransitionEnd',
    MozTransition: 'transitionend',
    WebkitTransition: 'webkitTransitionEnd',
  }
  let evt = null;
  for (const t in transitions) {
    if (el.style[t] !== undefined) {
      evt = transitions[t];
      break;
    }
  }
  return evt;
})();
export function transitionEnd(element, callback, flag) {
  if (flag) {
    element.removeEventListener(whichTransitionEvent, callback, false);
  } else {
    element.addEventListener(whichTransitionEvent, callback, false);
  }
}
