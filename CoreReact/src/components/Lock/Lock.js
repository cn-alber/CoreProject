import React from 'react'
import {message} from 'antd'
import classNames from 'classnames'
//import {endLoading} from 'utils'
import {ZPost} from 'utils/Xfetch'
import {connect} from 'react-redux'
import styles from './Lock.scss'

class Lock extends React.Component {

  constructor(props) {
    super(props)
    this.state = {
      loading: false,
      type: 'text'
    }
    this.timer = null
  }

  componentDidMount() {
    //endLoading()
  }

  componentWillUnmount() {
    this.ignorer = true
  }
  handleSubmit = (e) => {
    e.preventDefault()
    const password = this.refs.lockInput.value
    if (!password) {
      this.refs.lockInput.focus()
      message.error('请填写正确的解锁密码', 2)
      return false
    }
    this.setState({
      loading: true
    })
    const data = {
      password
    }
    return ZPost('profile/unlock', data, (s, d, m) => {
      this.props.dispatch({ type: 'LOCKED_SET', payload: false })
    }, () => {
      this.refs.lockInput.focus()
    }).then(() => {
      if (!this.ignorer) {
        this.setState({
          loading: false
        })
      }
    })
  }
  hackPWD = () => {
    if (this.state.type !== 'password') {
      this.setState({
        type: 'password'
      })
    }
  }
  render() {
    const {loading, type} = this.state
    const {username} = this.props
    const CN = classNames({
      [`${styles.loading}`]: loading
    })
    return (
      <div className={styles.wrapper}>
        <div className={styles.overlay} />
        <div className={styles.inner}>
          <div className={styles.zhang}>
            <div className={styles.chun}>
              <div />
            </div>
            <div className={styles.hua}>
              <form onSubmit={this.handleSubmit}>
                <label>{username}</label>
                <div className={CN} />
                <input disabled={loading} ref='lockInput' type={type} placeholder='解锁密码，回车确认' onFocus={this.hackPWD} autoComplete='off' />
              </form>
            </div>
          </div>
        </div>
      </div>
    )
  }
}

export default connect(state => ({
  username: state.user.name
}))(Lock)
