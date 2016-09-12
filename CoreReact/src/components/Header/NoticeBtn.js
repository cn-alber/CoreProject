import React from 'react'
import {Icon} from 'antd'
import styles from './Header.scss'
import {connect} from 'react-redux'

const NoticeBtn = React.createClass({

  getInitialState() {
    return {
      msgCount: 0
    }
  },
  componentDidMount() {
  },
  componentWillUnmount() {
  },
  openMsgWindow() {
    this.props.dispatch({ type: 'NOTICE_VISIBEL_REVER' })
  },
  render() {
    const {msgCount} = this.state
    return (
      <div className={styles.menuB} onClick={this.openMsgWindow}>
        <div className={styles.msger}>
          <Icon type='notification' />
          {msgCount > 0 && (
            <div className={styles.msgCount}>
              <span>{msgCount}</span>
            </div>
          )}
        </div>
      </div>
    )
  }
})

export default connect()(NoticeBtn)
