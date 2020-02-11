import React from 'react'
import { Provider } from 'react-redux'
import { store } from '../store'

import { Header } from "./header"
import { RebellionMap } from "./rebellion-map"

import styles from './app.module.scss'

export const App = () =>
  <div className={styles.app}>
    <Provider store={store}>
      <Header />
	    <RebellionMap/>
    </Provider>
  </div>
