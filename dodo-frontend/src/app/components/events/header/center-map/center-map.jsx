import React from 'react'
import PropTypes from 'prop-types'
import { Icon, Button } from 'app/components'

import styles from './center-map.module.scss'

export const CenterMap = ({ setCenterMap }) =>
	<Button onClick={() => setCenterMap(true)} variant="link" className={styles.button}>
		Recenter Map <Icon icon="bullseye" />
	</Button>

CenterMap.propTypes = {
	setCenterMap: PropTypes.func
}