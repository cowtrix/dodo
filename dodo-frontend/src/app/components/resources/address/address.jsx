import React from 'react'
import PropTypes from 'prop-types'

import styles from './address.module.scss'


export const Address = ({ address }) =>
	<div className={styles.address}>
		<p>{address.replace(',', '\n')}</p>
	</div>

Address.propTypes = {
	address: PropTypes.string,
}
