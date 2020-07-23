import React from 'react'
import PropTypes from 'prop-types'
import { Icon, Button } from 'app/components'

import styles from './center-map.module.scss'

export const CenterMap = ({ setCenterMap, display }) =>
	display ?
		<Button onClick={() => setCenterMap(true)} variant="link" className={styles.button}>
		Recenter Map <Icon icon="bullseye" />
	</Button> :
		null

CenterMap.propTypes = {
	setCenterMap: PropTypes.func,
	display: PropTypes.bool
}