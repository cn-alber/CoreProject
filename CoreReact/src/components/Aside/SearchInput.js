import React from 'react'
import {connect} from 'react-redux'
import { Input, Button, Icon } from 'antd'
import classNames from 'classnames'
const InputGroup = Input.Group

const SearchInput = React.createClass({
  getInitialState() {
    return {
      value: '',
      focus: false
    }
  },
  handleInputChange(e) {
    this.setState({
      value: e.target.value
    })
  },
  handleInputRemove(e) {
    this.setState({
      value: ''
    }, () => {
      this.props.dispatch({ type: 'PERMISSIONMENUFILTERNAME_SET', payload: this.state.value })
    })
  },
  handleFocusBlur(e) {
    this.setState({
      focus: e.target === document.activeElement
    })
  },
  handleSearch() {
    this.props.dispatch({ type: 'PERMISSIONMENUFILTERNAME_SET', payload: this.state.value })
  },
  render() {
    const { style, size, placeholder } = this.props
    const btnCls = classNames({
      'ant-search-btn': true,
      'ant-search-btn-noempty': !!this.state.value.trim()
    })
    const searchCls = classNames({
      'ant-search-input': true,
      'ant-search-input-focus': this.state.focus
    })
    return (
      <div className='ant-search-input-wrapper' style={style}>
        <InputGroup className={searchCls}>
          <Input placeholder={placeholder} value={this.state.value} onChange={this.handleInputChange}
            onFocus={this.handleFocusBlur} onBlur={this.handleFocusBlur} onPressEnter={this.handleSearch}
          />
        {this.state.value && <Icon type='cross-circle' onClick={this.handleInputRemove} />}
          <div className='ant-input-group-wrap'>
            <Button icon='search' className={btnCls} size={size} onClick={this.handleSearch} />
          </div>
        </InputGroup>
      </div>
    )
  }
})

export default connect()(SearchInput)
