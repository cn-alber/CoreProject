import React from 'react'
import { Form, Input, Modal } from 'antd'
import {connect} from 'react-redux'
import {ZPost} from 'utils/Xfetch'
const createForm = Form.create
const FormItem = Form.Item
function noop() {
  return false
}

const Demo = React.createClass({
  getInitialState() {
    return {
      confirmLoading: false
    }
  },
  handleSubmit() {
    this.props.form.validateFields((errors, values) => {
      if (!!errors) {
        return
      }
      this.setState({
        confirmLoading: true
      })
      ZPost('account/password', values, (s, d, m) => {
        this.hideModal()
      }).then(() => {
        this.setState({
          confirmLoading: false
        })
      })
    })
  },

  hideModal() {
    this.props.dispatch({ type: 'PROSET_VISIBEL_SET', payload: false })
    this.props.form.resetFields()
  },
  checkPass(rule, value, callback) {
    const { validateFields } = this.props.form
    if (value) {
      validateFields(['reNewPwd'], { force: true })
    }
    callback()
  },
  checkPass2(rule, value, callback) {
    const { getFieldValue } = this.props.form
    if (value && value !== getFieldValue('newPwd')) {
      callback('两次输入密码不一致！')
    } else {
      callback()
    }
  },
  render() {
    const { getFieldProps } = this.props.form
    const {visible} = this.props

    const formItemLayout = {
      labelCol: { span: 4 },
      wrapperCol: { span: 20 }
    }
    const oldPwdProps = getFieldProps('oldPwd', {
      rules: [
        { required: true, whitespace: true, message: '请填写旧密码' }
      ]
    })
    const newPwdProps = getFieldProps('newPwd', {
      rules: [
        { required: true, whitespace: true, min: 6, message: '请填写六位新密码' },
        { validator: this.checkPass }
      ]
    })
    const reNewPwdProps = getFieldProps('reNewPwd', {
      rules: [
        { required: true, whitespace: true, min: 6, message: '请再一次填写新密码' },
        { validator: this.checkPass2 }
      ]
    })
    return (
      <div>
        <Modal title='修改密码' visible={visible} onOk={this.handleSubmit} onCancel={this.hideModal} confirmLoading={this.state.confirmLoading}>
          <Form horizontal>
            <FormItem {...formItemLayout} label='旧&nbsp;密码'>
              <Input {...oldPwdProps} type='password' autoComplete='off' onContextMenu={noop} onPaste={noop} onCopy={noop} onCut={noop} />
            </FormItem>
            <FormItem {...formItemLayout} label='新&nbsp;密码'>
              <Input {...newPwdProps} type='password' autoComplete='off' onContextMenu={noop} onPaste={noop} onCopy={noop} onCut={noop} />
            </FormItem>
            <FormItem {...formItemLayout} label='重输新密码'>
              <Input {...reNewPwdProps} type='password' placeholder='两次输入密码保持一致' autoComplete='off' onContextMenu={noop} onPaste={noop} onCopy={noop} onCut={noop} />
            </FormItem>
          </Form>
        </Modal>
      </div>
    )
  }
})

export default connect(state => ({
  visible: state.proset_visibel
}))(createForm()(Demo))
