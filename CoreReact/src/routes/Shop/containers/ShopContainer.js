import React from 'react'
import {connect} from 'react-redux'
import classNames from 'classnames'
import styles from 'components/App.scss'

import {endLoading} from 'utils'
import store from 'utils/store'
import {ZGet} from 'utils/Xfetch'
import {loadData} from '../modules/actions'
import constants from 'constants'

import Side from './Side'
import Main from './Main'

class ShopContainer extends React.Component {

  constructor(props) {
    super(props)
    this.state = {
      loading: true
    }
  }
  componentWillMount = () => {
    ZGet('shop/preload', (s, d, m) => {
      const data = store.get(constants.StoreKeyA)
      if (typeof data !== 'undefined' && data.autoSave) {
        this.props.dispatch({type: 'SHOPCOLLAPSEA_SET', payload: !!data.collapse})
        this.props.dispatch({type: 'SHOPCONFIGA_MERGE', merge: data})
      }
      endLoading(() => {
        this.props.dispatch(loadData())
      })
    })
    // Request.get('test', (s, d, m) => {
    //   // this.setState(update(this.state, {
    //   //   proxyData: {
    //   //     $set: d,
    //   //   },
    //   // }));
    //   const table = d.datas ? (
    //     <Table columns={tableColumns} dataSource={d.datas} pagination={false} rowClassName={() => styles.tableRow} />
    //   ) : null;
    //   this.cache = {
    //     table,
    //   };
    //   this.setState({
    //     proxyData: {
    //       tableFlag: true,
    //     },
    //   });
    // });
  }
  componentDidMount = () => {}
  componentWillUnmount = () => {}

  //触发全部的刷新
  reloadAll = () => {}
  exportCSV = () => {
    const fields = ['car', 'price', 'color']
    const myCars = [
      {
        "car": "Audi",
        "price": 40000,
        "color": "blue"
      }, {
        "car": "BMW",
        "price": 35000,
        "color": "black"
      }, {
        "car": "Porsche",
        "price": 60000,
        "color": "green"
      }
    ];
    //http://stackoverflow.com/questions/3665115/create-a-file-in-memory-for-user-to-download-not-through-server
    //用这个来判断是否支持，不支持的话后端推送下载
    // try {
    //     var isFileSaverSupported = !!new Blob;
    // } catch (e) {}
    json2csv({
      data: myCars,
      fields
    }, (err, csv) => {
      if (err)
        console.log(err)
      const blob = new Blob([csv], {type: 'text/plain;charset=utf-8'})
      saveAs(blob, 'test.csv')
    })
  }
  render() {
    console.log(' -- app SHOP render...')
    const collapse = this.props.collapse
    const CN = classNames(styles.content, {
      [`${styles.collapse}`]: collapse
    })
    return (
      <div className={CN}>
        <Side />
        <Main />
      </div>
    )
  }
}

export default connect(state => ({collapse: state.ShopCollapseA}))(ShopContainer)
