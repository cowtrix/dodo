import React from 'react'
import { Provider } from 'react-redux'
import { store } from '../store'

import styles from './app.module.scss'
import { Dodo } from "./dodo";
import { initiateFontAwesomeLibrary } from './font-awesome-library'


export const App = () => {
	initiateFontAwesomeLibrary()

	return (
		<Provider store={store}>
			<div className={styles.app}>
				<Dodo/>
			</div>
		</Provider>
	)
}

