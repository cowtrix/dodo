import React, {Fragment} from 'react'
import PropTypes from 'prop-types'
import {SubHeader} from "app/components/sub-header"
import styles from './header.module.scss'


import {REBELLIONS_HEADER_COPY} from './constants'


export const Header = ({ rebellionsCount }) =>
    <div className={styles.header}>
        <SubHeader
            content={rebellionsCount + REBELLIONS_HEADER_COPY}
        />
    </div>

Header.propTypes = {
    rebellionsCount: PropTypes.number,
}