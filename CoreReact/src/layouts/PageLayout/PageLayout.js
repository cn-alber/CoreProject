import React from 'react'
import {endLoading} from 'utils'
import 'styles/core.scss'

const PageLayout = React.createClass({
  contextTypes: {
    router: React.PropTypes.object
  },
  getInitialState() {
    return {
      entering: true
    }
  },
  componentDidMount() {
    this.setState({
      entering: false
    })
    endLoading()
  },
  // componentWillReceiveProps(nextProps) {
  // },
  render() {
    if (this.state.entering) {
      return null
    }
    return this.props.children
  }
})
// export default connect(state => ({
//   authed: state.authed
// }))(PageLayout)

export default PageLayout
