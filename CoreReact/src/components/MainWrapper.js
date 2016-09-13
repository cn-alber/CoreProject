import React from 'react'
//import invariant from 'invariant'
//import hoistStatics from 'hoist-non-react-statics'
import EE from 'utils/EE'

const dataCB = 'refreshDataCallback'
// const getObjectClass = (obj) => {
//   if (typeof obj !== 'object' || obj === null) {
//     return false
//   }
//   return /(\w+)\(/.exec(obj.constructor.toString())[1]
// }
// const getDisplayName = (Component) => {
//   return Component.displayName || Component.name || 'Component'
// }

const MainWrapper = ComposedComponent => {
  //console.log(ComposedComponent.refreshDataCallback)
  return React.createClass({
    // constructor(props) {
    //   super(props)
    // },
    componentWillMount() {
      //console.warn('2222222222222222222222222222222 componentWillMount 1')
    },
    componentDidMount() {
      //console.info('componentDidMount', this.component)
      //console.warn('2222222222222222222222222222222 componentDidMount 2')
      EE.bindRefreshMain(this.component[dataCB])
      // console.group()
      // if (ComposedComponent._dataCache) {
      //   console.info(ComposedComponent._dataCache)
      // } else {
      //   ComposedComponent._dataCache = getObjectClass(this.component)
      //   console.warn(ComposedComponent._dataCache)
      // }
      // console.groupEnd()
    },
    componentWillUnmount() {
      //console.warn('2222222222222222222222222222222 componentWillUnmount 3')
      EE.offRefreshMain(this.component[dataCB])
    },
    handleChildRef(component) {
      //why undefined? maybe test error, what ever! checking by conditions
      this.component = component
      //console.warn('2222222222222222222222222222222 handleChildRef')
    },
    render() {
      return <ComposedComponent {...this.props} ref={this.handleChildRef} />
    }
  })
}
export default MainWrapper














//
// export default function MainWrapper(WrappedComponent, options = {}) {
//   // const {
//   //     withRef = false
//   // } = options
//   const WheatWrapper = React.createClass({
//     //displayName: `InjectIntl(${getDisplayName(WrappedComponent)})`,
//     //static WrappedComponent = WrappedComponent
//
//     // getWrappedInstance() {
//     //   invariant(withRef,
//     //       'To access the wrapped instance, ' +
//     //       'the `{withRef: true}` option must be set when calling: ' +
//     //       '`MainWrapper()`'
//     //   )
//     //   return this.refs.wrappedInstance
//     // }
//     componentWillMount() {
//       // if (!WrappedComponent.qq) {
//       //   WrappedComponent.qq = getDisplayName(WrappedComponent)
//       // } else {
//       //   console.error(WrappedComponent.qq)
//       // }
//       console.warn('componentWillMount 1')
//     },
//     componentDidMount() {
//       console.warn('componentDidMount 2')
//       //EE.bindRefreshMain(this.dataCBEvent)
//     },
//     componentWillUnmount() {
//       console.warn('componentWillUnmount 3')
//       EE.offRefreshMain(this.dataCBEvent)
//     },
//
//     handleRef(component) {
//       this.dataCBEvent = component ? component[dataCB] : null
//     },
//     //ref={withRef ? 'wrappedInstance' : this.handleRef}
//     render() {
//       console.warn('render')
//       return (
//         <WrappedComponent
//           {...this.props}
//           ref={this.handleRef}
//         />
//       )
//     }
//   })
//   return hoistStatics(WheatWrapper, WrappedComponent)
// }
