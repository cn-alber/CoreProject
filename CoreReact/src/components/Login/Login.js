import React from 'react'
import {Button, message, Popover} from 'antd'
import { Link } from 'react-router'
import {startLoading, endLoading} from 'utils'
import {ZPost} from 'utils/Xfetch'
import {COMPANY, BEIAN, SERVICE_CALL} from 'constants/config'
import styles from './Login.scss'

const helpContent = (
  <div>
    <p>1、如果没有企业账号，可以<Link to='/go/apply'>立即申请</Link></p>
    <p>2、忘记密码或其它事宜，请联系我们客服：{SERVICE_CALL[0]}</p>
  </div>
)

class Login extends React.Component {
  // static propTypes = {
  //   loginAsync: React.PropTypes.func.isRequired
  // }
  static contextTypes = {
    router: React.PropTypes.object
  }

  constructor(props) {
    super(props)
    this.state = {
      loading: false
    }
    this.timer = null
  }

  componentDidMount() {
    (() => {
      endLoading()
      runCanvas()
    })()
  }

  componentWillUnMount() {
    clearTimeout(this.timer)
  }
  timerOut(flag) {
    this.timer = setTimeout(() => {
      if (!flag) {
        this.refs.account.disabled = false
        this.refs.password.disabled = false
      }
      this.setState({
        loading: false
      })
    }, 450)
  }
  handleSubmit = (e) => {
    clearTimeout(this.timer)
    this.setState({
      loading: true
    })
    const account = this.refs.account.value
    const password = this.refs.password.value
    if (!account) {
      this.refs.account.focus()
      message.error('请正确填写账号', 2)
      this.timerOut(true)
      return
    }
    if (!password) {
      this.refs.password.focus()
      message.error('请正确填写密码', 2)
      this.timerOut(true)
      return
    }
    this.refs.account.disabled = true
    this.refs.password.disabled = true

    const vcode = ''
    const data = {
      account,
      password,
      vcode
    }
    return ZPost('sign/in', data, (s, d, m) => {
      this.context.router.push('/')
    }, () => {
      this.timerOut(false)
    })
  }
  render() {
    const {loading} = this.state
    return (
      <div className={styles.wrapper}>
        <div className={styles.inner}>
          <div className={styles.zhang}>
            <div className={styles.chun}>
              <div className={styles.logo}>&nbsp;</div>
            </div>
            <div className={styles.hua}>
              <div className={styles.input}>
                <input ref='account' type='text' placeholder='输入用户账号' />
              </div>
              <div className={styles.input}>
                <input ref='password' type='password' placeholder='输入登录密码' />
              </div>
              <div className={styles.btn}>
                <Button type='primary' loading={loading} onClick={this.handleSubmit}>确定</Button>
              </div>
              <div className={styles.help}>
                <Popover content={helpContent} trigger='hover'>
                  <a>账号有问题<em>?</em></a>
                </Popover>
              </div>
            </div>
          </div>
        </div>
        <canvas id='canvas' />
        <div className={styles.copyright}>&copy; {year} {COMPANY} {BEIAN}</div>
      </div>
    )
  }
}

export default Login

























const year = (() => {
  const date = new Date()
  return date.getFullYear()
})()
const runCanvas = function() {
  if (typeof document.getElementById('canvas').style.pointerEvents === 'undefined') {
    message.error('您的浏览器不支持系统使用，请尝试谷歌浏览器', 10)
  }
  let width
  let height
  let canvas
  let ctx
  let circles
  let animateHeader = true
  initHeader()
  addListeners()
  function initHeader() {
    width = window.innerWidth
    height = window.innerHeight
    canvas = document.getElementById('canvas')
    canvas.width = width
    canvas.height = height
    ctx = canvas.getContext('2d')
    circles = []
    for (let x = 0; x < 8; x++) {
      const c = new Circle()
      circles.push(c)
    }
    animate()
  }
  function addListeners() {
    window.addEventListener('scroll', scrollCheck)
    window.addEventListener('resize', resize)
  }
  function scrollCheck() {
    animateHeader = !(document.body.scrollTop > height)
  }
  function resize() {
    width = window.innerWidth
    height = window.innerHeight
    canvas.width = width
    canvas.height = height
  }
  function animate() {
    if (animateHeader) {
      ctx.clearRect(0, 0, width, height)
      for (var i in circles) {
        circles[i].draw()
      }
    }
    requestAnimationFrame(animate)
  }
  function Circle() {
    const init = () => {
      this.pos.x = Math.random() * width
      this.pos.y = height + Math.random() * 100
      this.alpha = 0.1 + Math.random() * 0.3
      this.scale = 0.1 + Math.random() * 0.3
      this.velocity = Math.random()
    }
    this.pos = {}
    init()
    this.draw = () => {
      if (this.alpha <= 0) {
        init()
      }
      this.pos.y -= this.velocity
      this.alpha -= 0.0005
      ctx.beginPath()
      ctx.arc(this.pos.x, this.pos.y, this.scale * 10, 0, 2 * Math.PI, false)
      ctx.fillStyle = 'rgba(255,255,255,' + this.alpha + ')'
      ctx.fill()
    }
  }
}
