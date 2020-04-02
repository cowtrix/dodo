import React from 'react'
import PropTypes from 'prop-types'
import { SubHeader, Icon } from "app/components"
import styles from './header.module.scss'

import { REBELLIONS_HEADER_COPY, LOCATION_SEARCH_COPY } from './constants'
import { Button } from 'app/components/button'


export const Header = ({ rebellionsCount }) =>
	<div className={styles.header}>
		<SubHeader
			content={rebellionsCount + REBELLIONS_HEADER_COPY}
		/>
		<Button onClick={() => {}}>
			{LOCATION_SEARCH_COPY}
			<Icon icon="bullseye" className={styles.searchIcon} size="s" />
		</Button>
	</div>

Header.propTypes = {
	rebellionsCount: PropTypes.number,
}
