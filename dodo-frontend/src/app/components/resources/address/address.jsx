import React from 'react'
import PropTypes from 'prop-types'

import styles from './address.module.scss'


export const Address = ({ address }) =>
	<div id="address" className={styles.address} dangerouslySetInnerHTML={{ __html: address.replace(new RegExp(',', 'g'), '<br />') } }></div>

Address.propTypes = {
	address: PropTypes.string,
}
