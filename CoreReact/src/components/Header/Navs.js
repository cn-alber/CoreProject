import React from 'react'
import styles from './Header.scss'
import Bookmark from './Bookmark'
import Operator from './Operator'

const Navs = React.createClass({
  render() {
    return (
      <div className={styles.navs}>
        <div className={styles.bookmarker}>
          <Bookmark />
        </div>
        <div className={styles.operator}>
          <Operator />
        </div>
      </div>
    )
  }
})

export default Navs
