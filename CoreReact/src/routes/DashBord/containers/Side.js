import React from 'react'
import styles from 'components/App.scss'

class Side extends React.Component {
  componentWillMount = () => {
  }
  componentDidMount = () => {
  }
  componentWillUnmount = () => {
  }

  render() {
    console.log(' -- component {Side} render...')

    return (
      <div className={styles.side}>
      </div>
    )
  }
}

export default Side
