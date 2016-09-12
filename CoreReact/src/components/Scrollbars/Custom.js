import React, { createClass } from 'react'
import { Scrollbars } from 'react-custom-scrollbars'
import './Custom.scss'

export default createClass({

  renderTrackHorizontal(props) {
    return (
      <div {...props} className='hua--track-horizontal' />
    )
  },
  renderTrackVertical(props) {
    return (
      <div {...props} className='hua--track-vertical' />
    )
  },
  renderThumbHorizontal(props) {
    return (
      <div {...props} className='hua--thumb-horizontal' />
    )
  },
  renderThumbVertical(props) {
    return (
      <div {...props} className='hua--thumb-vertical' />
    )
  },
  renderView(props) {
    return (
      <div {...props} className='hua--view' />
    )
  },
  render() {
    return (
      <Scrollbars
        renderTrackHorizontal={this.renderTrackHorizontal}
        renderTrackVertical={this.renderTrackVertical}
        renderThumbHorizontal={this.renderThumbHorizontal}
        renderThumbVertical={this.renderThumbVertical}
        renderView={this.renderView}
        className='hua--zhang'
        {...this.props} />
    )
  }
})
