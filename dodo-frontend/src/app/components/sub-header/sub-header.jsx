import React from 'react'
import PropTypes from 'prop-types'
import styles from './sub-header.module.scss'

export const SubHeader = ({ content}) =>
    <h2 className={styles.subHeader}>
        {content}
    </h2>

SubHeader.propTypes = {
    content: PropTypes.string,
}