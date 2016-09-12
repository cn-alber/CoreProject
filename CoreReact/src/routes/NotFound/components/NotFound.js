import React from 'react'
import styles from './NotFound.scss'
import {endLoading} from 'utils'

export const NotFound = () => {
  endLoading()
  return (
    <div className={styles.normal}>
      <div className={styles.container}>
        <h1 className={styles.title}>404</h1>
        <p className={styles.desc}>~页面内容不存在~</p>
      </div>
    </div>
  )
}

export default NotFound
