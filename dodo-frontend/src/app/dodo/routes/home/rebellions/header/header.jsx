import React from 'react'
import PropTypes from 'prop-types'
import { SubHeader } from "app/components/sub-header"
import styles from './header.module.scss'


import { REBELLIONS_HEADER_COPY } from './constants'
import { Button } from 'app/components/button'


export const Header = ({ rebellionsCount }) =>
	<div className={styles.header}>
		<SubHeader
			content={rebellionsCount + REBELLIONS_HEADER_COPY}
		/>
		<Button>Seach In My Location</Button>
	</div>

Header.propTypes = {
	rebellionsCount: PropTypes.number,
}