import React from 'react'
import {Icon} from 'antd'
import screenfull from 'screenfull'
import styles from './Header.scss'

const SF = React.createClass({

  getInitialState() {
    return {
      sfed: false
    }
  },
  componentDidMount() {
    if (screenfull.enabled) {
      document.addEventListener(screenfull.raw.fullscreenchange, this.fullscreenchange)
      // document.addEventListener(screenfull.raw.fullscreenerror, event => {
      //   console.error('Failed to enable fullscreen', event)
      // })
    }
  },
  componentWillUnmount() {
    if (screenfull.enabled) {
      document.removeEventListener(screenfull.raw.fullscreenchange, this.fullscreenchange)
    }
    //document.removeEventListener(screenfull.raw.fullscreenerror)
  },
  fullscreenchange() {
    this.setState({
      sfed: screenfull.isFullscreen
    })
  },
  handleFullScreen() {
    screenfull.toggle()
  },
  render() {
    const fsEnabled = screenfull.enabled
    if (!fsEnabled) {
      return null
    }
    const {sfed} = this.state
    return (
      <a className={styles.menuA} onClick={this.handleFullScreen}>
        {sfed ? (
          <Icon type='shrink' />
        ) : (
          <Icon type='arrow-salt' />
        )}
      </a>
    )
  }
})

export default SF
